using ArtVenue.Models;

namespace ArtVenue.ViewModels
{
    public class AdminManageModeratorsViewModel
    {
        public AppUser UserData { get; set; }

        //collection of users and roles
        public List<UserRole> UsersRoles { get; set; }

    }

    //connection between user and role
    public class UserRole
    {

        public string UserId { get; set; }

        public string UserName { get; set; }

        //true = is in role
        public bool IsSelected { get; set; }
    }
}
