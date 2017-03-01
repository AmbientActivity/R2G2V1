using System.Collections.Generic;

namespace Keebee.AAT.ApiClient.Models
{
    public class Config
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveEventLog { get; set; }
        public bool IsEventLogs { get; set; }
        public virtual IList<ConfigDetail> ConfigDetails { get; set; }
    }

    public class ConfigEdit
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveEventLog { get; set; }
    }

    public class ConfigList
    {
        public virtual IList<Config> Configs { get; set; }
    }
}
