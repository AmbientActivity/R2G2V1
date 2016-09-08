using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class Configuration
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public virtual IList<ConfigurationDetail> ConfigurationDetails { get; set; }
    }
}
