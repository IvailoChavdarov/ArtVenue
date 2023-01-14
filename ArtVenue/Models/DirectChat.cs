using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    //Added to optimize listing of direct chats of user 

    public class DirectChat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string FirstUserId { get; set; }
        public AppUser FirstUser { get; set; }
        [Required]
        public string SecondUserId { get; set; }
        public AppUser SecondUser { get; set; }
        public HashSet<Message> Messages { get; set; }
    }
}
