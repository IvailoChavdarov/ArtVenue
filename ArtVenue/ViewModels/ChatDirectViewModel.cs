using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ChatDirectViewModel : ViewModelWithChatsSidenav
    {
        //current user id for sending messages
        public string UserId { get; set; }

        //data for the user the current user chats with
        public string RecieverFirstName { get; set; }
        public string RecieverLastName { get; set; }
        public string RecieverId { get; set; }
        public string RecieverProfileImage { get; set; }

        //collection of the messages in the direct chat
        public List<Message> Messages { get; set; }
    }
}
