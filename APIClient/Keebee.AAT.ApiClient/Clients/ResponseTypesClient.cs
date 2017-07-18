﻿using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResponseTypesClient
    {
        IEnumerable<ResponseType> Get();
        IEnumerable<ResponseType> GetRandomTypes();
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

        public IEnumerable<ResponseType> GetRandomTypes()
        {
            var request = new RestRequest("responsetypes/randomtypes", Method.GET);
            var data = Execute(request);
            var responseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseType>>(data.Content);

            return responseTypes;
        }
    }
}
