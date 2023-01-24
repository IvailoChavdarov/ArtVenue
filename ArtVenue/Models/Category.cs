﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtVenue.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }
        public string? CategoryImage { get; set; }
        public string CategoryDescription { get; set; }

        public HashSet<Publications_Categories> Publications { get; set; }
        public HashSet<Users_Interests> Interested { get; set; }

        [NotMapped]
        public bool UserIsInterested { get; set; } = false;
    }
}
