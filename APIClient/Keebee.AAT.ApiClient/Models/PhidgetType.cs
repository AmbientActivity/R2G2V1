using System.Collections.Generic;

namespace Keebee.AAT.ApiClient
{
    public class PhidgetType
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class PhidgetTypeList
    {
        public IEnumerable<PhidgetType> PhidgetTypes;
    }
}
