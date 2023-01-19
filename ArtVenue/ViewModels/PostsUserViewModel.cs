using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsUserViewModel : ViewModelWithPublicationList
    {
        public AppUser User { get; set; }
        public bool IsTheUser { get; set; }
    }
}
