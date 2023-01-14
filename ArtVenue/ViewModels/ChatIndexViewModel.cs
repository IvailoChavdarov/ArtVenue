using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ChatIndexViewModel
    {
        public List<DirectChatCollectionItem> DirectChats { get;set; }
        public Dictionary<Group, Message> GroupChats { get; set; }
        public string UserId { get; set; }
    }
    public class DirectChatCollectionItem
    {
        public AppUser User { get; set; }
        public Message LastMessage { get; set; }
        public int ChatId { get; set; }
    }
}
