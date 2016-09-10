using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ConfigDetail
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Description { get; set; }
        public PhidgetType PhidgetType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class ConfigDetailEdit
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Description { get; set; }
        public int PhidgetTypeId { get; set; }
        public int ResponseTypeId { get; set; }
    }

    public class ConfigDetailList
    {
        public IEnumerable<ConfigDetail> ConfigDetails;
    }
}
