using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsIndexViewModel : ViewModelWithPublicationList
    {

    }
    public class PostCreator
    {
        public PostCreator(AppUser user)
        {
            this.FullName = user.FirstName + " " + user.LastName;
            this.UserId = user.Id;
            this.ProfileImage = user.GetProfileImage();
        }
        public string FullName { get; set; }
        public string UserId { get; set; }
        public string ProfileImage { get; set; }
    }
}
