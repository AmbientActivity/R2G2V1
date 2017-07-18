using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IInteractiveActivityTypesClient
    {
        IEnumerable<InteractiveActivityType> Get();
    }

    public class InteractiveActivityTypesClient : BaseClient, IInteractiveActivityTypesClient
    {
        public IEnumerable<InteractiveActivityType> Get()
        {
            var request = new RestRequest("interactiveactivitytypes", Method.GET);
            var data = Execute(request);
            var interactiveActivityTypes = JsonConvert.DeserializeObject<IEnumerable<InteractiveActivityType>>(data.Content);

            return interactiveActivityTypes;
        }
    }
}
