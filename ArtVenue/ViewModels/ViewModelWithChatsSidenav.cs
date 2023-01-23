using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ViewModelWithChatsSidenav
    {
        public List<DirectChatCollectionItem> DirectChats { get; set; }
        public Dictionary<Group, Message> GroupChats { get; set; }
        public string ChatWithId { get; set; }
        public int? ChatGroupId { get; set; }
    }
}
