using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsCategoryViewModel : ViewModelWithPublicationList
    {
        public Category Category { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsInterestedIn { get; set; } = false;
        public int CategoryToToggleId { get; set; } = 0;
    }
}
