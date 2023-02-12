using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    public class Category
    {
        //category identifiers
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        //additional info
        public string? CategoryImage { get; set; }
        public string CategoryDescription { get; set; }

        //relation properties
        public HashSet<Publications_Categories> Publications { get; set; }
        public HashSet<Users_Interests> Interested { get; set; }

        //used in controller to check if user has interest in given category
        [NotMapped]
        public bool UserIsInterested { get; set; } = false;
    }
}
