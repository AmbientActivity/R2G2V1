using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ProfileDetail
    {
        public int Id { get; set; }

        public int ActivityTypeId { get; set; }

        public ResponseType ResponseType { get; set; }

        public IEnumerable<Response> AmbientResponses { get; set; }

        public IEnumerable<Response> Responses { get; set; }
    }
}
