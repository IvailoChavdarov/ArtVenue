using ArtVenue.Data;
using ArtVenue.Hubs;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Text.RegularExpressions;

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
            List<DirectChatCollectionItem> directMessages = new List<DirectChatCollectionItem>();
            Dictionary<Models.Group, Message> groupMessages = new Dictionary<Models.Group, Message>();
            string userId = _userManager.GetUserId(User);
            foreach (var group_member in _db.Groups_Members.Where(x=>x.MemberId == userId))
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

                    groupMessages.Add(
                        _db.Groups.Where(x => x.Id == group_member.GroupId).First(),
                        lastMessage
                    );
                }
                else
                {
                    groupMessages.Add(
                        _db.Groups.Where(x => x.Id == group_member.GroupId).First(),
                        null);
                }
            }
            foreach (var directChat in _db.DirectChats.Where(x=>x.FirstUserId==userId||x.SecondUserId==userId)) {

                Message lastMessage = _db.Messages.Where(x => x.DirectChatId == directChat.Id).OrderByDescending(x => x.SendTime).First();
                AppUser chatWith;
                if (directChat.FirstUserId == userId)
                {
                    chatWith = await _userManager.FindByIdAsync(directChat.SecondUserId);
                }
                else
                {
                    chatWith = await _userManager.FindByIdAsync(directChat.FirstUserId);
                }
                directMessages.Add(new DirectChatCollectionItem
                {
                    ChatId=directChat.Id,
                    LastMessage=lastMessage,
                    User=chatWith
                });
            }
            groupMessages.OrderByDescending(x => x.Value.SendTime);
            directMessages.OrderByDescending(x => x.LastMessage.SendTime);
            ChatIndexViewModel data = new ChatIndexViewModel()
            {
                DirectChats = directMessages,
                GroupChats = groupMessages,
                UserId = userId
            };
            return View(data);
        }
        public async Task<IActionResult> Group(int id)
        {
            Dictionary<string, string> userNames = new Dictionary<string, string>();
            List<Message> messages = new List<Message>();
            foreach (var message in _db.Messages.Where(message => message.GroupId == id))
            {
                if (userNames.ContainsKey(message.SenderId))
                {
                    message.SenderName = userNames[message.SenderId];
                }
                else
                {
                    AppUser sender = await _userManager.FindByIdAsync(message.SenderId);
                    string senderFullName = sender.FirstName + " " + sender.LastName;
                    userNames.Add(message.SenderId, senderFullName);
                    message.SenderName = senderFullName;
                }
                messages.Add(message);
            }
            string userId = _userManager.GetUserId(User);
            GroupChatViewModel data = new GroupChatViewModel();
            data.Messages = messages;
            data.UserId = userId;
            data.GroupChatId = id.ToString();
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
            ChatDirectViewModel data = new ChatDirectViewModel()
            {
                Messages = messages,
                UserId = userId,
                RecieverFirstName= reciever.FirstName,
                RecieverLastName= reciever.LastName,
                RecieverId = reciever.Id,
                RecieverProfileImage = reciever.GetProfileImage()
            };
            return View(data);
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
    }
}
