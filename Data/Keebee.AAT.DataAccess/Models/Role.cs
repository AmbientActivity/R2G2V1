﻿using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
    }
}