using ArtVenue.Data;
using ArtVenue.Hubs;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace ArtVenue.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public ChatController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            ViewModelWithChatsSidenav userChats = await GetUserChats();
            ChatIndexViewModel data = new ChatIndexViewModel()
            {
                DirectChats = userChats.DirectChats,
                GroupChats = userChats.GroupChats,
                UserId = _userManager.GetUserId(User)
            };
            return View(data);
        }
        public async Task<IActionResult> Group(int id)
        {
            Dictionary<string, UsernameProfilePicPair> userNames = new Dictionary<string, UsernameProfilePicPair>();
            List<Message> messages = new List<Message>();
            string userId = _userManager.GetUserId(User);
            if (!_db.Groups_Members.Where(x=>x.GroupId==id && x.MemberId==userId).Any())
            {
                return Unauthorized();
            }
            foreach (var message in _db.Messages.Where(message => message.GroupId == id))
            {
                if (userNames.ContainsKey(message.SenderId))
                {
                    message.SenderName = userNames[message.SenderId].UserName;
                    message.SenderProfileImage = userNames[message.SenderId].UserPicture;
                }
                else
                {
                    AppUser sender = await _userManager.FindByIdAsync(message.SenderId);
                    string senderFullName = sender.FirstName + " " + sender.LastName;
                    string senderPicture = sender.GetProfileImage();
                    UsernameProfilePicPair pair = new UsernameProfilePicPair()
                    {
                        UserName = senderFullName,
                        UserPicture = senderPicture
                    };
                    userNames.Add(message.SenderId, pair);
                    message.SenderName = senderFullName;
                    message.SenderProfileImage = senderPicture;
                }
                messages.Add(message);
            }
            Group group = _db.Groups.Where(x => x.Id == id).FirstOrDefault();
            ViewModelWithChatsSidenav sidenavData = await GetUserChats();
            ChatGroupViewModel data = new ChatGroupViewModel();
            data.Messages = messages;
            data.UserId = userId;
            data.GroupChatId = id;
            data.GroupName = group.GroupName;
            data.GroupPicture = group.GetGroupPicture();
            data.GroupChats = sidenavData.GroupChats;
            data.DirectChats = sidenavData.DirectChats;
            return View(data);
        }
        public async Task<IActionResult> Direct(int id)
        {
            string userId = _userManager.GetUserId(User);
            DirectChat directChat = _db.DirectChats.Where(x=>x.Id==id).FirstOrDefault();
            AppUser reciever;
            if (directChat.FirstUserId == userId)
            {
                reciever = await _userManager.FindByIdAsync(directChat.SecondUserId);
            }
            else if(directChat.SecondUserId == userId)
            {
                reciever = await _userManager.FindByIdAsync(directChat.FirstUserId);
            }
            else
            {
                throw new Exception("Not Allowed");
            }
            List<Message> messages = _db.Messages.Where(x => x.DirectChatId == id).ToList();
            ViewModelWithChatsSidenav sidenavData = await GetUserChats();
            ChatDirectViewModel data = new ChatDirectViewModel()
            {
                Messages = messages,
                UserId = userId,
                RecieverFirstName= reciever.FirstName,
                RecieverLastName= reciever.LastName,
                RecieverId = reciever.Id,
                RecieverProfileImage = reciever.GetProfileImage(),
                DirectChats = sidenavData.DirectChats,
                GroupChats = sidenavData.GroupChats,
                ChatWithId = reciever.Id
            };
            return View(data);
        }
        public async Task<IActionResult> ChatWith(string id)
        {
            int chatId = await GetDirectChatWithId(id);

            return RedirectToAction("direct", new { id = chatId});
        }
        [HttpPost]
        public async Task<IActionResult> AddPersonToGroup(string userId, int groupId)
        {
            Groups_Members connection = new Groups_Members();
            connection.MemberId = userId;
            connection.GroupId = groupId;
            _db.Groups_Members.Add(connection);
            await _db.SaveChangesAsync();
            return View("Index");
        }
        private async Task<ViewModelWithChatsSidenav> GetUserChats()
        {
            List<DirectChatCollectionItem> directMessages = new List<DirectChatCollectionItem>();
            Dictionary<Models.Group, Message> groupMessages = new Dictionary<Models.Group, Message>();
            ViewModelWithChatsSidenav result = new ViewModelWithChatsSidenav();
            string userId = _userManager.GetUserId(User);
            foreach (var group_member in _db.Groups_Members.Where(x => x.MemberId == userId))
            {
                var allGroupMessages = _db.Messages.Where(x => x.GroupId == group_member.GroupId);
                if (allGroupMessages.Any())
                {
                    Message lastMessage = allGroupMessages.OrderByDescending(x => x.SendTime).First();

                    if (lastMessage.SenderId != userId)
                    {
                        AppUser lastMessageSender = await _userManager.FindByIdAsync(lastMessage.SenderId);
                        lastMessage.SenderName = lastMessageSender.FirstName + " " + lastMessageSender.LastName;
                    }
                    Group groupToAdd = _db.Groups.Where(x => x.Id == group_member.GroupId).FirstOrDefault();
                    groupToAdd.GroupPicture = groupToAdd.GetGroupPicture();
                    groupMessages.Add(
                        groupToAdd,
                        lastMessage
                    );
                }
                else
                {
                    Group groupToAdd = _db.Groups.Where(x => x.Id == group_member.GroupId).FirstOrDefault();
                    groupToAdd.GroupPicture = groupToAdd.GetGroupPicture();
                    groupMessages.Add(
                        groupToAdd,
                        null
                    );
                }
            }
            foreach (var directChat in _db.DirectChats.Where(x => x.FirstUserId == userId || x.SecondUserId == userId))
            {
                Message lastMessage = _db.Messages.Where(x => x.DirectChatId == directChat.Id).OrderByDescending(x => x.SendTime).FirstOrDefault();
                AppUser chatWith;
                if (directChat.FirstUserId == userId)
                {
                    chatWith = await _userManager.FindByIdAsync(directChat.SecondUserId);
                }
                else
                {
                    chatWith = await _userManager.FindByIdAsync(directChat.FirstUserId);
                }
                chatWith.ProfileImage = chatWith.GetProfileImage();
                directMessages.Add(new DirectChatCollectionItem
                {
                    ChatId = directChat.Id,
                    LastMessage = lastMessage,
                    User = chatWith
                });
            }
            groupMessages.OrderByDescending(x => x.Value.SendTime);
            directMessages.OrderByDescending(x => x.LastMessage.SendTime);
            result.DirectChats = directMessages;
            result.GroupChats = groupMessages;
            return result;
        }
        private async Task<int> GetDirectChatWithId(string recieverId)
        {
            string currentUserId = _userManager.GetUserId(User);
            var connection = _db.DirectChats
                .Where(x => (x.FirstUserId == recieverId && x.SecondUserId == currentUserId) || (x.FirstUserId == currentUserId && x.SecondUserId == recieverId));
            if (connection.Any())
            {
                return connection.First().Id;
            }
            else
            {
                DirectChat newConnection = new DirectChat()
                {
                    FirstUserId = currentUserId,
                    SecondUserId = recieverId
                };
                await _db.DirectChats.AddAsync(newConnection);
                _db.SaveChanges();
                return _db.DirectChats
                .Where(x => x.FirstUserId == currentUserId && x.SecondUserId == recieverId).First().Id;
            }
        }
    }
}
