using System;

namespace Keebee.AAT.RESTClient
{
    public class PersonalPicture
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public int ResidentId { get; set; }
        public string Filename { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
    }
}
