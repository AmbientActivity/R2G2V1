using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ConfigDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Config")]
        public int ConfigId { get; set; }
        public virtual Config Config { get; set; }

        [ForeignKey("ActivityType")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("ResponseType")]
        public int ResponseTypeId { get; set; }
        public virtual ResponseType ResponseType { get; set; }
    }
}