using System;
using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class MediaFile
    {
        [Key]
        public Guid StreamId { get; set; }
        public bool IsFolder { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public Int64? FileSize { get; set; }
        public string Path { get; set; }
    }
}