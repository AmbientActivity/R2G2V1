using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class Config
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveEventLog { get; set; }
        public virtual IList<ConfigDetail> ConfigDetails { get; set; }
    }
}