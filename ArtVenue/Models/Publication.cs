using ArtVenue.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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

        [Display(Name = "Publication title")]
        public string PublicationTitle { get; set; }
        [Display(Name = "Publication description")]
        public string PublicationText { get; set; }
        
        public int? GroupId { get; set; }
        [Display(Name = "Image")]
        public string ImageLink { get; set; }
        [Display(Name = "Video")]
        public string VideoLink { get; set; }
        public bool HasManyImages { get; set; }
        [Display(Name = "Link to YouTube video embed")]
        public string EmbeddedVideoLink { get; set; }
        [Required]
        public string PostedTime { get; set; }
        [NotMapped]
        public PostCreator PostedBy { get; set; }
        public AppUser Creator { get; set; }
        public Group Group { get; set; }
        public HashSet<GalleryImage> Gallery { get; set; }
        public HashSet<Publications_Categories> Categories { get; set; }
        public HashSet<Users_Saved> SavedBy { get; set; }
        public HashSet<Comment> Comments { get; set; }
        [NotMapped]
        public List<string> Images { get; set; }
        [NotMapped]
        public List<Comment> PostComments { get; set; }
        [NotMapped]
        public List<int> CategoriesIds { get; set; }
        [NotMapped]
        public bool IsSavedByUser { get; set; }
    }
}
