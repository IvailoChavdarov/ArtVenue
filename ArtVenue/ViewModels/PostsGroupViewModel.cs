using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsGroupViewModel : ViewModelWithPublicationList
    {
        public Group Group { get; set; }
        public bool HasRequestedToJoin { get; set; }
        public bool IsInGroup { get; set; }
        public bool HasAccess { get; set; }
        public bool IsGroupCreator { get; set; }
    }
}
