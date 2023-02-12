using ArtVenue.Data;
using ArtVenue.Hubs;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
            //gets user chats
            ViewModelWithChatsSidenav userChats = await GetUserChats();

            //sets data to model
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
            //optimizes the connection between message and sender by storing user id, to not search for same user data multiple times
            Dictionary<string, UsernameProfilePicPair> userNames = new Dictionary<string, UsernameProfilePicPair>();

            List<Message> messages = new List<Message>();

            string userId = _userManager.GetUserId(User);

            //validates that user has access to the given group chat
            if (!_db.Groups_Members.Where(x=>x.GroupId==id && x.MemberId==userId).Any())
            {
                return Forbid();
            }

            //gets group messages data
            foreach (var message in _db.Messages.Where(message => message.GroupId == id))
            {
                //connects message and sender
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


            //gets group data
            Group group = _db.Groups
                .Where(x => x.Id == id)
                .FirstOrDefault();

            //gets user's chat to show in sidenav
            ViewModelWithChatsSidenav sidenavData = await GetUserChats();

            //sets data to model
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

            //gets direct chat
            DirectChat directChat = await _db.DirectChats
                .Where(x=>x.Id==id)
                .FirstOrDefaultAsync();

            //gets remote user data
            AppUser reciever;

            //checks sets remote user data
            if (directChat.FirstUserId == userId)
            {
                reciever = await _userManager.FindByIdAsync(directChat.SecondUserId);
            }
            else if(directChat.SecondUserId == userId)
            {
                reciever = await _userManager.FindByIdAsync(directChat.FirstUserId);
            }
            //validates that the current user has access to chat
            else
            {
                return Forbid();
            }

            //gets direct chat messages
            List<Message> messages = await _db.Messages
                .Where(x => x.DirectChatId == id)
                .ToListAsync();

            //gets user's chat to show in sidenav
            ViewModelWithChatsSidenav sidenavData = await GetUserChats();

            //sets data to model
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
            //gets id of direct chat
            int chatId = await GetDirectChatWithId(id);

            //redirects to direct chat page
            return RedirectToAction("direct", new { id = chatId});
        }

        private async Task<ViewModelWithChatsSidenav> GetUserChats()
        {
            //stores direct chats
            List<DirectChatCollectionItem> directMessages = new List<DirectChatCollectionItem>();


            //stores group chats key=group value=last message
            Dictionary<Models.Group, Message> groupMessages = new Dictionary<Models.Group, Message>();

            //connects direct and group chats
            ViewModelWithChatsSidenav result = new ViewModelWithChatsSidenav();

            string userId = _userManager.GetUserId(User);

            //gets user's groups chats
            foreach (var group_member in _db.Groups_Members.Where(x => x.MemberId == userId))
            {

                var allGroupMessages = _db.Messages
                  .Where(x => x.GroupId == group_member.GroupId);

                //checks if there are any messages in group chat
                if (allGroupMessages.Any())
                {
                    //sets last message
                    Message lastMessage = allGroupMessages
                        .OrderByDescending(x => x.SendTime)
                        .First();

                    if (lastMessage.SenderId != userId)
                    {
                        //sets last message sender
                        AppUser lastMessageSender = await _userManager.FindByIdAsync(lastMessage.SenderId);
                        lastMessage.SenderName = lastMessageSender.FirstName + " " + lastMessageSender.LastName;
                    }

                    //gets group data
                    Group groupToAdd = _db.Groups
                        .Where(x => x.Id == group_member.GroupId)
                        .FirstOrDefault();

                    groupToAdd.GroupPicture = groupToAdd.GetGroupPicture();

                    //sets value in dictionary
                    groupMessages.Add(groupToAdd, lastMessage);
                }
                else
                {
                    //sets group chat data without any messages
                    Group groupToAdd = _db.Groups
                        .Where(x => x.Id == group_member.GroupId)
                        .FirstOrDefault();

                    groupToAdd.GroupPicture = groupToAdd.GetGroupPicture();
                    groupMessages.Add(
                        groupToAdd,
                        null
                    );
                }
            }

            foreach (var directChat in _db.DirectChats.Where(x => x.FirstUserId == userId || x.SecondUserId == userId))
            {
                //gets last message with user
                Message lastMessage = _db.Messages
                    .Where(x => x.DirectChatId == directChat.Id)
                    .OrderByDescending(x => x.SendTime)
                    .FirstOrDefault();

                //gets remote user data
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

                //adds direct chat data to collection
                directMessages.Add(new DirectChatCollectionItem
                {
                    ChatId = directChat.Id,
                    LastMessage = lastMessage,
                    User = chatWith
                });
            }

            //sorts chats by last activity
            groupMessages.OrderByDescending(x => x.Value.SendTime);
            directMessages.OrderByDescending(x => x.LastMessage.SendTime);

            //sets data to result
            result.DirectChats = directMessages;
            result.GroupChats = groupMessages;

            return result;
        }
        private async Task<int> GetDirectChatWithId(string recieverId)
        {
            string currentUserId = _userManager.GetUserId(User);

            //gets direct chat id
            var connection = _db.DirectChats
                .Where(x => 
                    (x.FirstUserId == recieverId && x.SecondUserId == currentUserId) ||
                    (x.FirstUserId == currentUserId && x.SecondUserId == recieverId)
                 );

            //checks if direct chat exists
            if (connection.Any())
            {
                //returns direct chat id
                return connection.First().Id;
            }
            else
            {
                //creates direct chat if it doesnt exist
                DirectChat newConnection = new DirectChat()
                {
                    FirstUserId = currentUserId,
                    SecondUserId = recieverId
                };
                await _db.DirectChats.AddAsync(newConnection);
                await _db.SaveChangesAsync();

                var createdDirectChatId = _db.DirectChats
                    .First(x => x.FirstUserId == currentUserId && x.SecondUserId == recieverId)
                    .Id;

                //returns new direct chat id
                return createdDirectChatId;
            }
        }
    }
}
