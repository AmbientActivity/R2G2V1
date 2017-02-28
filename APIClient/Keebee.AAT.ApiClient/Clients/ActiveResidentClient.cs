using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IActiveResidentClient
    {
        ActiveResident Get();
        void Patch(ActiveResidentEdit resident);
    }

    public class ActiveResidentClient :  BaseClient, IActiveResidentClient
    {
        public ActiveResident Get()
        {
            var request = new RestRequest("activeresidents/1", Method.GET);
            var data = Execute(request);
            var resident = JsonConvert.DeserializeObject<ActiveResident>(data.Content);

            return resident;
        }

        public void Patch(ActiveResidentEdit resident)
        {
            var request = new RestRequest("activeresidents/1", Method.PATCH);
            var json = request.JsonSerializer.Serialize(resident);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            Execute(request);
        }
    }
}
