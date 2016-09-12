using System;
using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class MediaFileStream
    {
        public Guid StreamId { get; set; }
        public bool IsFolder { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
        public byte[] Stream{ get; set; }
    }

    public class MediaFileStreamList
    {
        public IEnumerable<MediaFileStream> Media;
    }
}
