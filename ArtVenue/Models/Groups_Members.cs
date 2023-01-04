﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    public class Groups_Members
    {
        [Key]
        public string MemberId { get; set; }
        public AppUser Member { get; set; }

        [Key]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        [Required]
        [MaxLength(25)]
        public string JoinedDate { get; set; }
    }
}
