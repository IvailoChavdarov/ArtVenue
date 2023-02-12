using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ChatIndexViewModel
    {
        //collection of all current user's direct chats
        public List<DirectChatCollectionItem> DirectChats { get;set; }

        //collection of all groups and last messages of group chat the current user participates in
        public Dictionary<Group, Message> GroupChats { get; set; }

        //the current user id for finding direct chats id
        public string UserId { get; set; }
    }
    public class DirectChatCollectionItem
    {
        //connection between user the current user has chat, the chat id and the last message of the chat
        public AppUser User { get; set; }
        public Message LastMessage { get; set; }
        public int ChatId { get; set; }
    }
}
