using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostDetailsViewModel
    {
        //publication data
        public Publication Post { get; set; }

        //publication creator data
        public AppUser Creator { get; set; }

        //collection of all post's categories
        public List<Category> Categories{ get; set; }
    }
}
