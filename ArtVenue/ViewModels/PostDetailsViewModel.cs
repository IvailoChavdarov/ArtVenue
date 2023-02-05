using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostDetailsViewModel
    {
        public Publication Post { get; set; }
        public AppUser Creator { get; set; }
        public List<Category> Categories{ get; set; }
    }
}
