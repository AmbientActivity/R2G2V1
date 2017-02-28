using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResponseTypesClient
    {
        IEnumerable<ResponseType> Get();
    }

    public class ResponseTypesClient : BaseClient, IResponseTypesClient
    {
        public IEnumerable<ResponseType> Get()
        {
            var request = new RestRequest("responsetypes", Method.GET);
            var data = Execute(request);
            var responseTypes = JsonConvert.DeserializeObject<ResponseTypeList>(data.Content).ResponseTypes;

            return responseTypes;
        }
    }
}
