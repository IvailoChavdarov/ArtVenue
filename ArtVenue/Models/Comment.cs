using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ArtVenue.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int PublicationId { get; set; }
        [Required]
        public string CommentContent { get; set; }
        [Required]
        public string PostedTime { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        public AppUser User { get; set; }
        public Publication Publication { get; set; }
    }
}
