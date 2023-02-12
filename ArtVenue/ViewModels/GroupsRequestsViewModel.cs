using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class GroupsRequestsViewModel
    {
        //the name of the group
        public string GroupName { get; set; }

        //collection of all the group's join requests
        public IEnumerable<Groups_Requests> Requests { get; set; }
    }
}
