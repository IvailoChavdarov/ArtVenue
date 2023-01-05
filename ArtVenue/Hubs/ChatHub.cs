using ArtVenue.Data;
using ArtVenue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
            message.SendTime = DateTime.Now.ToString();
            message.Sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = message.Sender.FirstName + " " + message.Sender.LastName;
            await Clients.User(recieverId).SendAsync("ReceiveMessage", message);
            message.RecieverId = recieverId;
            AddMessageToDatabase(message);
        }
        public async Task SendMessage(Message message)
        {
            message.SendTime = DateTime.Now.ToString();
            message.Sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = message.Sender.FirstName + " " + message.Sender.LastName;
            await Clients.All.SendAsync("ReceiveMessage", message);
            AddMessageToDatabase(message);
        }
        public async Task SendMessageToGroup(Message message, int groupId)
        {
            message.SendTime = DateTime.Now.ToString();
            message.Sender = await _userManager.FindByIdAsync(message.SenderId);
            message.SenderName = message.Sender.FirstName + " " + message.Sender.LastName;
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
    }
}