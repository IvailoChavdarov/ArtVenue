using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class AdminUsersViewModel
    {
        //colleciton of all users registered on the website and their roles
        public List<MinifiedUser> Users { get; set; }
    }
    public class MinifiedUser
    {
        //minified data of user and his roles
        public string Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }
        public string UserName { get; set; }
    }
    public class MinifiedGroup
    {
        //minigied data of group
        public int Id { get; set; }
        public string GroupName { get; set; }
    }
}
