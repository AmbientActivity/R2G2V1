using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IMediaPathTypesClient
    {
        IEnumerable<MediaPathType> Get();
        MediaPathType Get(int mediaPathTypeId);

        IEnumerable<MediaPathType> Get(bool isSystem);
    }

    public class MediaPathTypesClient : IMediaPathTypesClient
    {
        private readonly ClientBase _clientBase;

        public MediaPathTypesClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<MediaPathType> Get()
        {
            var data = _clientBase.Get("mediapathtypes");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var pathTypes = serializer.Deserialize<MediaPathTypeList>(data).MediaPathTypes;

            return pathTypes;
        }

        public MediaPathType Get(int mediaPathTypeId)
        {
            var data = _clientBase.Get($"mediapathtypes/{mediaPathTypeId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var pathType = serializer.Deserialize<MediaPathType>(data);

            return pathType;
        }

        public IEnumerable<MediaPathType> Get(bool isSystem)
        {
            var data = _clientBase.Get($"mediapathtypes?isSystem={isSystem}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var pathTypes = serializer.Deserialize<MediaPathTypeList>(data).MediaPathTypes;

            return pathTypes;
        }
    }
}
