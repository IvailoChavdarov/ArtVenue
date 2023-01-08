using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    public class Publications_Categories
    {
        [Key]
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }

        [Key]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
