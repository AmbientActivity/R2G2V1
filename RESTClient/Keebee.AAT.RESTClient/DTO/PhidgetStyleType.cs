using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class PhidgetStyleType
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class PhidgetStyleTypeList
    {
        public IEnumerable<PhidgetStyleType> PhidgetStyleTypes;
    }
}
