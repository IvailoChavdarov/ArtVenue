using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsCategoryViewModel : ViewModelWithPublicationList
    {
        //data for the category
        public Category Category { get; set; }

        //true - the user is authenticated
        public bool IsLoggedIn { get; set; }

        //true - the user has selected this category for his interest
        public bool IsInterestedIn { get; set; } = false;

        //connection between view and controller for adding and removing interests
        public int CategoryToToggleId { get; set; } = 0;
    }
}
