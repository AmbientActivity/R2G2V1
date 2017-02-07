using System;
using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class MediaFile
    {
        public Guid StreamId { get; set; }
        public bool IsFolder { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public bool IsShared { get; set; }
    }

    public class LinkedMediaFile : MediaFile
    {
        public int Id { get; set; }
    }

    public class MediaFileSingle : MediaFile
    {
        public string Path { get; set; }
    }

    public class MediaFileStreamSingle : MediaFile
    {
        public byte[] Stream{ get; set; }
    }

    public class MediaFilePath
    {
        public MediaPathType MediaPathType { get; set; }
        public IEnumerable<LinkedMediaFile> Files;
    }

    public class MediaResponseType
    {
        public ResponseType ResponseType { get; set; }
        public IEnumerable<MediaFilePath> Paths;
    }

    public class MediaResponseTypeList
    {
        public IEnumerable<MediaResponseType> Media;
    }

    // when accessing the media files controller directly
    public class Media
    {
        public string Path { get; set; }
        public IEnumerable<MediaFile> Files;
    }

    public class MediaList
    {
        public IEnumerable<Media> Media;
    }
}
