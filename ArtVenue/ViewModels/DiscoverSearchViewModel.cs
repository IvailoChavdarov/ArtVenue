using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class DiscoverSearchViewModel
    {
        public string SearchType { get; set; }
        public string SearchQuery { get; set; }
        public List<SearchResultItem> ResultItems { get; set; }
    }
    public class SearchResultItem
    {
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
        public string Title { get; set; }
        public int Id { get; set; }
        public string UserId { get; set; }
    }
}
