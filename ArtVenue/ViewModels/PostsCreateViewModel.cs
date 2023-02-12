using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class PostsCreateViewModel
    {
        //collection of all categories on the website for the publication
        public List<Category> Categories { get; set; }

        //collection of user's groups for user to select targeted group
        public List<Group> UserGroups { get; set; }

        //the new publication data
        public Publication PublicationToPost { get; set; }

        //the ids of the categories of the publication
        public int[] PublicationToPostCategoriesIds { get; set; } = new int[3];

        //the url's of the publication images(if the user is creating publication with multiple images)
        public string[] PublicationToPostImages { get; set; } = new string[5];
    }
}
