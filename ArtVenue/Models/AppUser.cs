using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ArtVenue.Models
{
    public class AppUser: IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public bool IsPublic { get; set; }
        public string? ProfileImage { get; set; }
        public HashSet<Message> Messages_Sent { get; set; }
        public HashSet<Group> GroupsCreated { get; set; }
        public HashSet<Groups_Members> GroupsJoined { get; set; }
        public HashSet<Publication> PublicationsPosted { get; set; }
        public HashSet<Users_Interests> Interests { get; set; }
        public HashSet<Users_Saved> Saved { get; set; }
        public HashSet<Comment> Comments { get; set; }
        public HashSet<DirectChat> DirectChats { get; set; }
        public HashSet<DirectChat> DirectChatsSecondUser { get; set; }
        public string GetProfileImage()
        {
            if (string.IsNullOrEmpty(this.ProfileImage))
            {
                return "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1673793962/profile-placeholder_ivroyn.png";
            }
            else
            {
                return this.ProfileImage;
            }
        }
    }
}
