using System;

namespace Keebee.AAT.ApiClient.Models
{
    public class PublicMediaFile
    {
        public int Id { get; set; }
        public MediaFile MediaFile { get; set; }
        public MediaPathType MediaPathType { get; set; }
        public ResponseType ResponseType { get; set; }
        public DateTime DateAdded { get; set; }
    }

    public class PublicMediaFileEdit
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public int MediaPathTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public bool IsLinked { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
