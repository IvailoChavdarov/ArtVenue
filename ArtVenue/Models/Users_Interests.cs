using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    public class Users_Interests
    {
        [Key]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        [Key]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
