using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IMediaPathTypesClient
    {
        IEnumerable<MediaPathType> Get();
        MediaPathType Get(int mediaPathTypeId);

        IEnumerable<MediaPathType> Get(bool isSystem);
    }

    public class MediaPathTypesClient : BaseClient, IMediaPathTypesClient
    {
        public IEnumerable<MediaPathType> Get()
        {
            var request = new RestRequest("mediapathtypes", Method.GET);
            var data = Execute(request);
            var pathTypes = JsonConvert.DeserializeObject<MediaPathTypeList>(data.Content).MediaPathTypes;

            return pathTypes;
        }

        public MediaPathType Get(int mediaPathTypeId)
        {
            var request = new RestRequest($"mediapathtypes/{mediaPathTypeId}", Method.GET);
            var data = Execute(request);
            var pathType = JsonConvert.DeserializeObject<MediaPathType>(data.Content);

            return pathType;
        }

        public IEnumerable<MediaPathType> Get(bool isSystem)
        {
            var request = new RestRequest($"mediapathtypes?isSystem={isSystem}", Method.GET);
            var data = Execute(request);
            var pathTypes = JsonConvert.DeserializeObject<MediaPathTypeList>(data.Content).MediaPathTypes;

            return pathTypes;
        }
    }
}
