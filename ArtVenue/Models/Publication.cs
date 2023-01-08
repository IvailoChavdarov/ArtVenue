using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArtVenue.Models
{
    public class Publication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string CreatorId { get; set; }
        public string PublicationText { get; set; }
        
        public int? GroupId { get; set; }
        
        public string ImageLink { get; set; }
        public string VideoLink { get; set; }
        public bool HasManyImages { get; set; }
        public string OutsideLink { get; set; }
        public string EmbeddedVideoLink { get; set; }
        [Required]
        public string PostedTime { get; set; }
        [NotMapped]
        public string CreatorName { get; set; }
        public AppUser Creator { get; set; }
        public Group Group { get; set; }
        public HashSet<GalleryImage> Gallery { get; set; }
        public HashSet<Publications_Categories> Categories { get; set; }
        public HashSet<Users_Saved> SavedBy { get; set; }
        public HashSet<Comment> Comments { get; set; }
    }
}
