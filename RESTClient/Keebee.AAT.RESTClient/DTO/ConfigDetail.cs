using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ConfigDetail
    {
        public int Id { get; set; }
        public int ConfigurationId { get; set; }
        public ActivityType ActivityType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class ConfigDetailList
    {
        public IEnumerable<ConfigDetail> ConfigDetails;
    }
}
