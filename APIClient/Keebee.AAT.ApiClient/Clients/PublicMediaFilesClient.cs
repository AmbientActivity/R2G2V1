using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IPublicMediaFilesClient
    {
        PublicMedia Get();
        PublicMediaFile Get(int id);
        PublicMedia Get(bool isSystem);
        PublicMediaResponseType GetForResponseType(int responseTypeId);
        PublicMedia GetForMediaPathType(int mediaPathTypeId);
        PublicMediaFile GetForResponseTypeFilename(int responseTypeId, string filename);
        IEnumerable<PublicMediaFile> GetForStreamId(Guid streamId);
        PublicMedia GetLinkedForStreamId(Guid streamId);
        int[] GetIdsForStreamId(Guid streamId);
        PublicMedia GetLinked();
        int Post(PublicMediaFileEdit publicMediaFile);
        string Delete(int id);
    }

    public class PublicMediaFilesClient : IPublicMediaFilesClient
    {
        private readonly ClientBase _clientBase;

        public PublicMediaFilesClient()
        {
            _clientBase = new ClientBase();
        }

        public PublicMediaFile Get(int id)
        {
            var data = _clientBase.Get($"publicmediafiles/{id}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaFile = serializer.Deserialize<PublicMediaFile>(data);

            return mediaFile;
        }

        public PublicMedia Get()
        {
            var data = _clientBase.Get("publicmediafiles");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMedia>(data);

            return publicMedia;
        }

        public PublicMedia Get(bool isSystem)
        {
            var data = _clientBase.Get($"publicmediafiles?isSystem={isSystem}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMedia>(data);

            return publicMedia;
        }

        public PublicMediaResponseType GetForResponseType(int responseTypeId)
        {
            var data = _clientBase.Get($"publicmediafiles?responseTypeId={responseTypeId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMediaResponseType>(data);

            return publicMedia;
        }

        public PublicMedia GetForMediaPathType(int mediaPathTypeId)
        {
            var data = _clientBase.Get($"publicmediafiles?mediaPathTypeId={mediaPathTypeId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMedia>(data);

            return publicMedia;
        }

        public IEnumerable<PublicMediaFile> GetForStreamId(Guid streamId)
        {
            var data = _clientBase.Get($"publicmediafiles?streamId={streamId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaStreamIds = serializer.Deserialize<PublicMediaStreamIdList>(data).MediaFiles;

            return mediaStreamIds;
        }

        public PublicMediaFile GetForResponseTypeFilename(int responseTypeId, string filename)
        {
            var data = _clientBase.Get($"publicmediafiles?responseTypeId={responseTypeId}&filename={filename}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaFile = serializer.Deserialize<PublicMediaFile>(data);

            return mediaFile;
        }

        public PublicMedia GetLinked()
        {
            var data = _clientBase.Get("publicmediafiles/linked");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var media = serializer.Deserialize<PublicMedia>(data);

            return media;
        }

        public PublicMedia GetLinkedForStreamId(Guid streamId)
        {
            var data = _clientBase.Get($"publicmediafiles/linked?streamId={streamId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var media = serializer.Deserialize<PublicMedia>(data);

            return media;
        }

        public int[] GetIdsForStreamId(Guid streamId)
        {
            var data = _clientBase.Get($"publicmediafiles/ids?streamId={streamId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var ids = serializer.Deserialize<int[]>(data);

            return ids;
        }

        public int Post(PublicMediaFileEdit publicMediaFile)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(publicMediaFile);

            return _clientBase.Post("publicmediafiles", el);
        }

        public string Delete(int id)
        {
            return _clientBase.Delete($"publicmediafiles/{id}");
        }
    }
}
