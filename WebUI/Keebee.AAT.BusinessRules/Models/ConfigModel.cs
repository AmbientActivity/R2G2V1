using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;

namespace Keebee.AAT.BusinessRules.Models
{
    public class ConfigModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public ConfigDetail ConfigDetail { get; set; }
        public IEnumerable<PhidgetType> PhidgetTypes { get; set; }
        public IEnumerable<PhidgetStyleType> PhidgetStyleTypes { get; set; }
        public IEnumerable<ResponseType> ResponseTypes { get; set; }
    }
}
