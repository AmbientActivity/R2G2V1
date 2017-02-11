using System.Collections.Generic;

namespace Keebee.AAT.ApiClient
{
    public class MediaPathType
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public bool IsPreviewable { get; set; }
        public bool IsSystem { get; set; }
        public bool IsLinkable { get; set; }
    }

    public class MediaPathTypeList
    {
        public IEnumerable<MediaPathType> MediaPathTypes;
    }
}
