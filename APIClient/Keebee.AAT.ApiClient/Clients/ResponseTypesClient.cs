using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResponseTypesClient
    {
        IEnumerable<ResponseType> Get();
        ResponseType Get(int id);
        IEnumerable<ResponseType> GetRandomTypes();
        IEnumerable<ResponseType> GeRotationalTypes();
    }

    public class ResponseTypesClient : BaseClient, IResponseTypesClient
    {
        public IEnumerable<ResponseType> Get()
        {
            var request = new RestRequest("responsetypes", Method.GET);
            var data = Execute(request);
            var responseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseType>>(data.Content);

            return responseTypes;
        }

        public ResponseType Get(int id)
        {
            var request = new RestRequest($"responsetypes/{id}", Method.GET);
            var data = Execute(request);
            var responseType = JsonConvert.DeserializeObject<ResponseType>(data.Content);

            return responseType;
        }

        public IEnumerable<ResponseType> GetRandomTypes()
        {
            var request = new RestRequest("responsetypes/random", Method.GET);
            var data = Execute(request);
            var responseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseType>>(data.Content);

            return responseTypes;
        }

        public IEnumerable<ResponseType> GeRotationalTypes()
        {
            var request = new RestRequest("responsetypes/rotational", Method.GET);
            var data = Execute(request);
            var responseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseType>>(data.Content);

            return responseTypes;
        }
    }
}
