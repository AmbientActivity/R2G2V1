using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class Config
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public virtual IList<ConfigDetail> ConfigDetails { get; set; }
    }

    public class ConfigEdit
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ConfigList
    {
        public virtual IList<Config> Configs { get; set; }
    }
}
