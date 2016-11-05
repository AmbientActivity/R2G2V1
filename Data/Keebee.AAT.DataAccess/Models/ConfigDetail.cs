using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ConfigDetail
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }

        [ForeignKey("Config")]
        public int ConfigId { get; set; }
        public virtual Config Config { get; set; }

        [ForeignKey("PhidgetType")]
        public int PhidgetTypeId { get; set; }
        public virtual PhidgetType PhidgetType { get; set; }

        [ForeignKey("PhidgetStyleType")]
        public int PhidgetStyleTypeId { get; set; }
        public virtual PhidgetStyleType PhidgetStyleType { get; set; }

        [ForeignKey("ResponseType")]
        public int ResponseTypeId { get; set; }
        public virtual ResponseType ResponseType { get; set; }
    }
}