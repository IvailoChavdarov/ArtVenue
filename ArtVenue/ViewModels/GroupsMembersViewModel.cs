using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class GroupsMembersViewModel
    {
        public string GroupName { get; set; }
        public IEnumerable<Groups_Members> Memberships { get; set; }
    }
}
