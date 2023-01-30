using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsCreateViewModel
    {
        public List<Category> Categories { get; set; }

        public List<Group> UserGroups { get; set; }

        public Publication PublicationToPost { get; set; }
        public int[] PublicationToPostCategoriesIds { get; set; } = new int[3];

        public string[] PublicationToPostImages { get; set; } = new string[5];
    }
}
