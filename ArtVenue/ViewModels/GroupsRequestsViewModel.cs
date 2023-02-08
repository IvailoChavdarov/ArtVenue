using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class GroupsRequestsViewModel
    {
        public string GroupName { get; set; }
        public IEnumerable<Groups_Requests> Memberships { get; set; }
    }
}
