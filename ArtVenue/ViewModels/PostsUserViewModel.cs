using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsUserViewModel : ViewModelWithPublicationList
    {
        //user data
        public AppUser User { get; set; }

        //true - the user searched for is the current user
        public bool IsTheUser { get; set; }
    }
}
