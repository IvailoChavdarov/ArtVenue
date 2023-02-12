using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ChatGroupViewModel:ViewModelWithChatsSidenav
    {
        //collection of all messages in group chat
        public List<Message> Messages { get; set; }

        //current user data for sending messages
        public string UserId { get; set; }

        //data for the group and group chat
        public int GroupChatId { get; set; }
        public string GroupPicture { get; set; }
        public string GroupName { get; set; }
    }
    public class UsernameProfilePicPair
    {
        //connection between user's full name and picture
        public string UserName { get; set; }
        public string UserPicture { get; set; }
    }
}
