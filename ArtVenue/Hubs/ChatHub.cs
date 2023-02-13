using ArtVenue.Data;
using ArtVenue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace ArtVenue.Hubs
{
    public class ChatHub : Hub
    {
        //dependency injection
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        //send message in direct chat and saves it in database by reciever id and message data
        public async Task SendMessageToUser(Message message, string recieverId)
        {
            //sets message send time to now
            message.SendTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture);

            //sets message sender to current user
            AppUser sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = sender.FirstName;
            message.SenderProfileImage = sender.GetProfileImage();

            //sends message to remote user for instant JS rendering of message
            await Clients.User(recieverId).SendAsync("ReceiveMessage", message);

            //sets message chat to current chat between users
            message.DirectChatId = await GetDirectChatId(message.SenderId, recieverId);

            //adds message to database
            AddMessageToDatabase(message);
        }

        //send message in group chat and saves it in database by group id and message data
        public async Task SendMessageToGroup(Message message, int groupId)
        {
            //sets message send time to now
            message.SendTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture);

            //sets message sender to current user
            message.Sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = message.Sender.FirstName + " " + message.Sender.LastName;
            message.IsFromCurrentUser = true;
            message.SenderProfileImage = message.Sender.GetProfileImage();

            //sends message to current chat group for instant JS rendering of message
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", message);

            message.GroupId = groupId;

            //adds message to database
            AddMessageToDatabase(message);
        }

        //connects current user to group chat instant rendering
        public async Task AddToGroupInstantChat(int groupId)
        {
            //adds user to chat group instant message rendering
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        //adds given message to database
        public void AddMessageToDatabase(Message message)
        {
            _db.Messages.Add(message);
            _db.SaveChangesAsync();
        }

        //gets direct chat id by the two user ids
        public async Task<int> GetDirectChatId(string senderId, string recieverId)
        {
            var chats = _db.DirectChats
                .Where(x => x.FirstUserId == senderId && x.SecondUserId == recieverId);

            DirectChat chat;

            if (chats.Any())
            {
                chat = await chats.FirstAsync();
            }
            else
            {
                chat = await _db.DirectChats
                    .Where(x => x.SecondUserId == senderId && x.FirstUserId == recieverId)
                    .FirstOrDefaultAsync();

                if (chat==null)
                {
                    throw new ArgumentException("Chat does not exist");
                }
            }

            return chat.Id;
        }
    }
}