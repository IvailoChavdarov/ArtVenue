using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class ViewModelWithPublicationList : ViewModelWithCommentInput
    {
        public List<Publication> Publications { get; set; } = new List<Publication>();
        public string UserProfilePicture { get; set; }
        public string UserName { get; set; }
        public bool NextPage { get; set; }
        public bool PrevPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
