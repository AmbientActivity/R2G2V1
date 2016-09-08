using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ConfigurationDetail
    {
        public int Id { get; set; }
        public int ConfigurationId { get; set; }
        public ActivityType ActivityType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class ConfigurationDetailList
    {
        public IEnumerable<ConfigurationDetail> ConfigurationDetails;
    }
}
