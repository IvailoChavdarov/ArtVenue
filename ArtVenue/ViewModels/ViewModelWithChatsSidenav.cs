using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    //chats base ViewModel used to display sidenav with chats
    public class ViewModelWithChatsSidenav
    {
        //collection of user's direct chats
        public List<DirectChatCollectionItem> DirectChats { get; set; }

        //collection of user's direct chats
        public Dictionary<Group, Message> GroupChats { get; set; }

        //checks if user is in the chat with selected other user
        public string ChatWithId { get; set; }

        //if current user is writing in group chat
        public int? ChatGroupId { get; set; }
    }
}
