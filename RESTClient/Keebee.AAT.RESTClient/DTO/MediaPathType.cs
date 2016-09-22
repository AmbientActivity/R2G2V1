using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class MediaPathType
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class MediaPathTypeList
    {
        public IEnumerable<MediaPathType> MediaPathTypes;
    }
}
