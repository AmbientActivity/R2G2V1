using System;
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

    public class PublicMediaResponseType
    {
        public MediaResponseType MediaResponseType;
    }

    public class PublicMediaFileEdit
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public int MediaPathTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public bool IsShared { get; set; }
    }

    public class PublicMediaStreamIdList
    {
        public IEnumerable<PublicMediaFile> MediaFiles;
    }
}
