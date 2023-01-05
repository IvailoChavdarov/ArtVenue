using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class GroupChatViewModel
    {
        public List<Message> Messages { get; set; }
        public string UserId { get; set; }
        public string GroupChatId { get; set; }
    }
}
