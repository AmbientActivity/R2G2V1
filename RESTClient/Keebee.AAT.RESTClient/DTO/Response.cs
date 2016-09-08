using System;
using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class Response
    {
        public int Id { get; set; }
        public int ResponseTypeId { get; set; }
        public int ProfileId { get; set; }
        public Guid StreamId { get; set; }
        public string Filename { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
    }

    public class ResponseList
    {
        public IEnumerable<Response> AmbientResponses;
    }

}
