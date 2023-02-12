using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    //base class for displaying list of publications and user and groups sidenav and managing pages of action
    public class ViewModelWithPublicationList : ViewModelWithCommentInput
    {
        //collection of sorted publications
        public List<Publication> Publications { get; set; } = new List<Publication>();

        //current user data for sidenav
        public string UserProfilePicture { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }

        //current user's groups for group sidenav
        public List<Group> Groups { get; set; } = new List<Group>();

        //managing pages of list with publications
        public bool NextPage { get; set; }
        public bool PrevPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
