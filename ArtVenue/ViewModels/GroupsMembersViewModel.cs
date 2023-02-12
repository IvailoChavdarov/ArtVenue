using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class GroupsMembersViewModel
    {
        //the name of the group
        public string GroupName { get; set; }

        //collection of all the group's members
        public IEnumerable<Groups_Members> Memberships { get; set; }
    }
}
