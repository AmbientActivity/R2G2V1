using System;
using System.Collections.Generic;

namespace Keebee.AAT.ApiClient
{
    public class SystemMediaFile
    {
        public int Id { get; set; }
        public MediaFile MediaFile { get; set; }
        public MediaPathType MediaPathType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class SystemMedia
    {
        public IEnumerable<MediaResponseType> MediaFiles;
    }

    public class SystemMediaResponseType
    {
        public MediaResponseType MediaResponseType;
    }

    public class SystemMediaFileEdit
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public int MediaPathTypeId { get; set; }
        public int ResponseTypeId { get; set; }
    }

    public class SystemMediaStreamIdList
    {
        public IEnumerable<SystemMediaFile> MediaFiles;
    }
}
