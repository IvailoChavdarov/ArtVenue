using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
            Dictionary<string, string> userNames = new Dictionary<string, string>();
            List<Message> messages = new List<Message>();
            foreach (var message in _db.Messages)
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
            ChatViewModel data = new ChatViewModel();
            data.Messages = messages;
            data.UserId = userId;
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
    }
}
