using System;
using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ResidentMediaFile
    {
        public int Id { get; set; }
        public MediaFile MediaFile { get; set; }
        public Resident Resident { get; set; }
        public MediaPathType MediaPathType  { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class ResidentMediaResponseTypeSingle
    {
        public ResidentMediaResponseType ResidentMedia;
    }

    public class ResidentMediaResponseType
    {
        public Resident Resident { get; set; }
        public MediaResponseType MediaResponseType;
    }

    public class ResidentMedia
    {
        public Resident Resident { get; set; }
        public IEnumerable<MediaResponseType> MediaResponseTypes;
    }

    public class ResidentMediaSingle
    {
        public ResidentMedia ResidentMedia;
    }

    public class ResidentMediaList
    {
        public IEnumerable<ResidentMedia> Media;
    }

    public class ResidentMediaFileEdit
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public int ResidentId { get; set; }
        public int MediaPathTypeId { get; set; }
        public int ResponseTypeId { get; set; }
    }
}
