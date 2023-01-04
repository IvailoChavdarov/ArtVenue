using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArtVenue.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string SenderId { get; set; }
        [Required]
        public string MessageContent { get; set; }
        [AllowNull]
        public string RecieverId { get; set; }

        [Required]
        public string SendTime { get; set; }

        [NotMapped]
        public string SenderName { get; set; }
        public AppUser Sender { get; set; }
        public AppUser Reciever { get; set; }
    }
}
