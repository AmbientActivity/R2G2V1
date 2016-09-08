using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ResponseType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsInteractive{ get; set; }
        public bool IsSystem { get; set; }
        public ResponseTypeCategory ResponseTypeCategory { get; set; }
        public IEnumerable<Response> Responses { get; set; }
    }
}
