using System;
using System.Collections.Generic;

namespace Keebee.AAT.ApiClient.Models
{
    public class MediaFile
    {
        public Guid StreamId { get; set; }
        public bool IsFolder { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public bool IsLinked { get; set; }
        public DateTime DateAdded { get; set; }
        public int NumLinkedProfiles { get; set; }
    }

    public class MediaFileLinked : MediaFile
    {
        public int Id { get; set; }
    }

    public class MediaFilePath : MediaFile
    {
        public string Path { get; set; }
    }

    public class MediaFileStream : MediaFile
    {
        public string Path { get; set; }
        public byte[] Stream { get; set; }
    }

    public class MediaFileThumbnail : MediaFile
    {
        public byte[] Thumbnail { get; set; }
    }

    public class MediaPathTypeFiles
    {
        public MediaPathType MediaPathType { get; set; }
        public IEnumerable<MediaFileLinked> Files;
    }

    public class ResponseTypePaths
    {
        public ResponseType ResponseType { get; set; }
        public IEnumerable<MediaPathTypeFiles> Paths;
    }

    // when accessing the media files controller directly
    public class Media
    {
        public string Path { get; set; }
        public IEnumerable<MediaFile> Files;
    }
}
