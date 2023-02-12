using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class GroupsIndexViewModel
    {
        //collection of all groups the user has created
        public List<Group> GroupsCreated { get; set; }

        //collection of all groups the user has joined and not created
        public List<Group> GroupsJoined { get; set; }
    }
}
