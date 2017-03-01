using System.Collections.Generic;

namespace Keebee.AAT.ApiClient.Models
{
    public class ConfigDetail
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsEventLogs { get; set; }
        public PhidgetType PhidgetType { get; set; }
        public PhidgetStyleType PhidgetStyleType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class ConfigDetailEdit
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int PhidgetTypeId { get; set; }
        public int PhidgetStyleTypeId { get; set; }
        public int ResponseTypeId { get; set; }
    }

    public class ConfigDetailList
    {
        public IEnumerable<ConfigDetail> ConfigDetails;
    }
}
