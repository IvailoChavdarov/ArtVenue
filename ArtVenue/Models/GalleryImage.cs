using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ArtVenue.Models
{
    public class GalleryImage
    {
        //identifiers

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string ImageLink { get; set; }
        [Required]
        public int PublicationId { get; set; }

        //relation property
        public Publication Publication { get; set; }
    }
}
