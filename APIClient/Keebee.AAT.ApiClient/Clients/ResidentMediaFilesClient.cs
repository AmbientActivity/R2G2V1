using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResidentMediaFilesClient
    {
        IEnumerable<ResidentMedia> Get();
        ResidentMediaFile Get(int id);
        ResidentMedia GetForResident(int residentId);
        ResidentMediaResponseType GetForResidentResponseType(int residentId, int responseTypeId);
        IEnumerable<ResidentMedia> GetLinked();
        IEnumerable<ResidentMedia> GetLinkedForStreamId(Guid streamId);
        int[] GetIdsForStreamId(Guid streamId);
        int Post(ResidentMediaFileEdit residenttMediaFile);
        string Delete(int id);
    }

    public class ResidentMediaFilesClient : IResidentMediaFilesClient
    {
        private readonly ClientBase _clientBase;

        public ResidentMediaFilesClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<ResidentMedia> Get()
        {
            var data = _clientBase.Get("residentmediafiles");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residentMedia = serializer.Deserialize<ResidentMediaResponseTypeList>(data).ResidentMediaList;

            return residentMedia;
        }

        public ResidentMediaFile Get(int id)
        {
            var data = _clientBase.Get($"residentmediafiles/{id}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residentMediaFile = serializer.Deserialize<ResidentMediaFile>(data);

            return residentMediaFile;
        }

        public ResidentMedia GetForResident(int residentId)
        {
            var data = _clientBase.Get($"residentmediafiles?residentId={residentId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residentMedia = serializer.Deserialize<ResidentMediaSingle>(data).ResidentMedia;

            return residentMedia;
        }

        public ResidentMediaResponseType GetForResidentResponseType(int residentId, int responseTypeId)
        {
            var data = _clientBase.Get($"residentmediafiles?residentId={residentId}&responseTypeId={responseTypeId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaResponseType = serializer.Deserialize<ResidentMediaResponseTypeSingle>(data).ResidentMedia;

            return mediaResponseType;
        }

        public IEnumerable<ResidentMedia> GetLinked()
        {
            var data = _clientBase.Get("residentmediafiles/linked");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaList = serializer.Deserialize<ResidentMediaResponseTypeList>(data).ResidentMediaList;

            return mediaList;
        }

        public IEnumerable<ResidentMedia> GetLinkedForStreamId(Guid streamId)
        {
            var data = _clientBase.Get($"residentmediafiles/linked?streamId={streamId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaList = serializer.Deserialize<ResidentMediaResponseTypeList>(data).ResidentMediaList;

            return mediaList;
        }

        public int[] GetIdsForStreamId(Guid streamId)
        {
            var data = _clientBase.Get($"residentmediafiles/ids?streamId={streamId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var ids = serializer.Deserialize<int[]>(data);

            return ids;
        }

        public int Post(ResidentMediaFileEdit residenttMediaFile)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(residenttMediaFile);

            return _clientBase.Post("residentmediafiles", el);
        }

        public string Delete(int id)
        {
            return _clientBase.Delete(string.Format($"residentmediafiles/{id}"));
        }
    }
}
