using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArtVenue.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string GroupName { get; set; }
        [AllowNull]
        public string Description { get; set; }
        [Required]
        public bool IsPrivate { get; set; }
        [AllowNull]
        public string GroupPicture { get; set; }
        [AllowNull]
        public string GroupBackground { get; set; }
        [Required]
        public string CreatorId { get; set; }
        public AppUser Creator { get; set; }
        public HashSet<Groups_Members> Memberships { get; set; }
        public HashSet<Message> Messages { get; set; }
        public HashSet<Publication> Publications { get; set; }
    }
}
