using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class DiscoverSearchViewModel
    {
        //the data the user has searched
        public string SearchType { get; set; }
        public string SearchQuery { get; set; }

        //collection of the results for the search
        public List<SearchResultItem> ResultItems { get; set; }
    }
    public class SearchResultItem
    {
        //constructors varying on the search type
        public SearchResultItem(Category category)
        {
            Title = category.CategoryName;
            Id= category.Id;
        }
        public SearchResultItem(Group group)
        {
            Title = group.GroupName;
            Id = group.Id;
        }
        public SearchResultItem(AppUser user)
        {
            Title = user.FirstName+" "+user.LastName;
            UserId = user.Id;
        }

        //the user/category/group name
        public string Title { get; set; }

        //if the search is for group or category
        public int Id { get; set; }

        //if the search is for users
        public string UserId { get; set; }
    }
}
