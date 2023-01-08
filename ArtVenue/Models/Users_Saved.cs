using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    public class Users_Saved
    {
        [Key]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        [Key]
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }
    }
}
