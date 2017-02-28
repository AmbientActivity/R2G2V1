using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IPhidgetTypesClient
    {
        IEnumerable<PhidgetType> Get();
    }

    public class PhidgetTypesClient : BaseClient, IPhidgetTypesClient
    {
        public IEnumerable<PhidgetType> Get()
        {
            var request = new RestRequest("phidgettypes", Method.GET);
            var data = Execute(request);
            var phidgetTypes = JsonConvert.DeserializeObject<PhidgetTypeList>(data.Content).PhidgetTypes;

            return phidgetTypes;
        }
    }
}
