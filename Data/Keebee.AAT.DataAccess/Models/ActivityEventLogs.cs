﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ActivityEventLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Resident")]
        public int? ResidentId { get; set; }
        public virtual Resident Resident { get; set; }

        [ForeignKey("Config")]
        public int ConfigId { get; set; }
        public virtual Config Config { get; set; }

        [ForeignKey("PhidgetType")]
        public int PhidgetTypeId { get; set; }
        public virtual PhidgetType PhidgetType { get; set; }

        [ForeignKey("ResponseType")]
        public int ResponseTypeId { get; set; }
        public virtual ResponseType ResponseType { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DateEntry { get; set; }
    }
}