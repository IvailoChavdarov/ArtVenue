using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArtVenue.Models
{
    public class Message
    {
        //identifiers
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string MessageContent { get; set; }

        //if message is for direct chat
        public int? DirectChatId { get; set; }

        //if its message in group chat
        public int? GroupId { get; set; }

        [Required]
        public string SendTime { get; set; }

        //used by controllers to fill data for sender
        [NotMapped]
        public string SenderName { get; set; }

        [NotMapped]
        public string SenderProfileImage { get; set; }

        [NotMapped]
        public bool IsFromCurrentUser { get; set; } = false;

        //relation properties
        public AppUser Sender { get; set; }
        public DirectChat DirectChat { get; set; }
        public Group Group { get; set; }
    }
}
