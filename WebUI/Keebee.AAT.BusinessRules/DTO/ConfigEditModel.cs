using Keebee.AAT.RESTClient;
using System.Collections.Generic;

namespace Keebee.AAT.BusinessRules.DTO
{
    public class ConfigEditModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public ConfigDetail ConfigDetail { get; set; }
        public IEnumerable<PhidgetType> PhidgetTypes { get; set; }
        public IEnumerable<ResponseType> ResponseTypes { get; set; }
    }
}
