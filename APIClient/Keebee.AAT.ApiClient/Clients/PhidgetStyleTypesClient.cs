using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IPhidgetStyleTypesClient
    {
        IEnumerable<PhidgetStyleType> Get();
    }

    public class PhidgetStyleTypesClient : BaseClient, IPhidgetStyleTypesClient
    {
        public IEnumerable<PhidgetStyleType> Get()
        {
            var request = new RestRequest("phidgetstyletypes", Method.GET);
            var data = Execute(request);
            var phidgetTypes = JsonConvert.DeserializeObject<PhidgetStyleTypeList>(data.Content).PhidgetStyleTypes;

            return phidgetTypes;
        }
    }
}
