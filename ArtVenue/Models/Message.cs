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
        public int? DirectChatId { get; set; }
        [AllowNull]
        public int? GroupId { get; set; }

        [Required]
        public string SendTime { get; set; }

        [NotMapped]
        public string SenderName { get; set; }
        public AppUser Sender { get; set; }
        public DirectChat DirectChat { get; set; }
        public Group Group { get; set; }
    }
}
