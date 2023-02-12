using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArtVenue.Models
{
    public class AppUser: IdentityUser
    {
        //additional user info for social media's work
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        //additional non-required user info
        public string? ProfileImage { get; set; }
        public string? ProfileBackground { get; set; }
        public string? ArtType { get; set; }
        public string? Bio { get; set; }
        public string? OutsideLink { get; set; }

        //relation properties
        public HashSet<Message> Messages_Sent { get; set; }
        public HashSet<Group> GroupsCreated { get; set; }
        public HashSet<Groups_Members> GroupsJoined { get; set; }
        public HashSet<Groups_Requests> GroupsJoinRequested { get; set; }
        public HashSet<Publication> PublicationsPosted { get; set; }
        public HashSet<Users_Interests> Interests { get; set; }
        public HashSet<Users_Saved> Saved { get; set; }
        public HashSet<Comment> Comments { get; set; }
        public HashSet<DirectChat> DirectChats { get; set; }
        public HashSet<DirectChat> DirectChatsSecondUser { get; set; }

        //gets profile image if user has set specific one or returns placeholder image
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
