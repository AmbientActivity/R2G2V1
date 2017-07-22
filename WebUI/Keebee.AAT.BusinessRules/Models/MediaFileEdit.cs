using System;

namespace Keebee.AAT.BusinessRules.Models
{
    public class MediaFileEdit
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
        public int MediaPathTypeId { get; set; }
        public bool IsLinked { get; set; }
        public DateTime DateAdded { get; set; }
        public string Thumbnail { get; set; }
    }

    public class SharedMediaFileEdit
    {
        public Guid StreamId { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
        public int MediaPathTypeId { get; set; }
        public int NumLinkedProfiles { get; set; }
        public string Thumbnail { get; set; }
    }
}
