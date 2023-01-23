using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ChatGroupViewModel:ViewModelWithChatsSidenav
    {
        public List<Message> Messages { get; set; }
        public string UserId { get; set; }
        public int GroupChatId { get; set; }
        public string GroupPicture { get; set; }
        public string GroupName { get; set; }
    }
    public class UsernameProfilePicPair
    {
        public string UserName { get; set; }
        public string UserPicture { get; set; }
    }
}
