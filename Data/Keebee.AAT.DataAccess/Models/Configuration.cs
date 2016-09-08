using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class Configuration
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public virtual IList<ConfigurationDetail> ConfigurationDetails { get; set; }
    }
}