using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsGroupViewModel : ViewModelWithPublicationList
    {
        //group data
        public Group Group { get; set; }

        //true - the current user has requested to join the private group
        public bool HasRequestedToJoin { get; set; }

        //true - the current user is member of the group
        public bool IsInGroup { get; set; }

        //true - the current user has access to see the publications in the given group
        public bool HasAccess { get; set; }

        //true - the current user has created the given group
        public bool IsGroupCreator { get; set; }
    }
}
