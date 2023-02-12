using ArtVenue.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ArtVenue.Models
{
    public class Comment
    {
        //identifiers
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int PublicationId { get; set; }

        //content
        [Required]
        public string CommentContent { get; set; }
        [Required]
        public string PostedTime { get; set; }

        //properties used by controller to fill data for sender
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public PostCreator Sender { get; set; }

        //relation properties
        public AppUser User { get; set; }
        public Publication Publication { get; set; }

    }
}
