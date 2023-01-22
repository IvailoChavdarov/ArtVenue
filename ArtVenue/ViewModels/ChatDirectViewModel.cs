using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ChatDirectViewModel
    {
        public string UserId { get; set; }
        public string RecieverFirstName { get; set; }
        public string RecieverLastName { get; set; }
        public string RecieverId { get; set; }
        public string RecieverProfileImage { get; set; }
        public List<Message> Messages { get; set; }
    }
}
