using ArtVenue.Data;
using ArtVenue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using System.Security.Claims;

namespace ArtVenue.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task SendMessageToUser(Message message, string recieverId)
        {
            message.SendTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture);
            AppUser sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = sender.FirstName;
            message.SenderProfileImage = sender.GetProfileImage();
            await Clients.User(recieverId).SendAsync("ReceiveMessage", message);
            message.DirectChatId = await GetDirectChatId(message.SenderId, recieverId);
            AddMessageToDatabase(message);
        }
        public async Task SendMessageToGroup(Message message, int groupId)
        {
            message.SendTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture);
            message.Sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = message.Sender.FirstName + " " + message.Sender.LastName;
            message.IsFromCurrentUser = true;
            message.SenderProfileImage = message.Sender.GetProfileImage();
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", message);
            message.GroupId = groupId;
            AddMessageToDatabase(message);
        }
        public async Task AddToGroupInstantChat(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }
        public void AddMessageToDatabase(Message message)
        {
            _db.Messages.Add(message);
            _db.SaveChanges();
        }
        public async Task<int> GetDirectChatId(string senderId, string recieverId)
        {
            var chats = _db.DirectChats.Where(x => x.FirstUserId == senderId && x.SecondUserId == recieverId);
            DirectChat chat;
            if (chats.Any())
            {
                chat = chats.First();
            }
            else
            {
                chat = _db.DirectChats.Where(x => x.SecondUserId == senderId && x.FirstUserId == recieverId).First();
            }

            return chat.Id;
        }
    }
}