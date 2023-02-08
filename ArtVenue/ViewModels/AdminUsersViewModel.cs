using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class AdminUsersViewModel
    {
        public List<MinifiedUser> Users { get; set; }
    }
    public class MinifiedUser
    {
        public string Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }
        public string UserName { get; set; }
    }
    public class MinifiedGroup
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
    }
}
