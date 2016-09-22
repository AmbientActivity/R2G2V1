using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class PublicMediaFile
    {
        public int Id { get; set; }
        public MediaFile MediaFile { get; set; }
        public MediaPathType MediaPathType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class PublicMedia
    {
        public IEnumerable<MediaResponseType> MediaFiles;
    }
}
