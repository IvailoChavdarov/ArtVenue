using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class AdminManageModeratorsViewModel
    {
        public AppUser UserData { get; set; }
        public List<UserRole> UsersRoles { get; set; }

    }
    public class UserRole
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
