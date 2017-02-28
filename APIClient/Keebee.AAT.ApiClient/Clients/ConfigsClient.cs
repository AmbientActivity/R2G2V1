using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IConfigsClient
    {
        IEnumerable<Config> Get();
        Config Get(int id);
        Config GetActive();
        Config GetByDescription(string description);
        Config GetWithDetails(int id);
        Config GetActiveDetails();
        int Activate(int id);
        IEnumerable<ConfigDetail> GetDetails();
        ConfigDetail GetDetail(int detailId);
        int Post(ConfigEdit config);
        int PostDetail(ConfigDetailEdit configDetail);
        void Patch(int id, ConfigEdit config);
        void PatchDetail(int detailId, ConfigDetailEdit configDetail);
        string Delete(int id);
        string DeleteDetail(int detailId);
    }

    public class ConfigsClient : BaseClient, IConfigsClient
    {
        public IEnumerable<Config> Get()
        {
            var request = new RestRequest("configs", Method.GET);
            var data = Execute(request);
            var configs = JsonConvert.DeserializeObject<ConfigList>(data.Content).Configs;

            return configs;
        }

        public Config Get(int id)
        {
            var request = new RestRequest($"configs/{id}", Method.GET);
            var data = Execute(request);
            var config = JsonConvert.DeserializeObject<Config>(data.Content);

            return config;
        }

        public Config GetActive()
        {
            var request = new RestRequest("configs/active", Method.GET);
            var data = Execute(request);
            var config = JsonConvert.DeserializeObject<Config>(data.Content);

            return config;
        }

        public Config GetByDescription(string description)
        {
            var request = new RestRequest($"configs?description={description}", Method.GET);
            var data = Execute(request);
            var config = JsonConvert.DeserializeObject<Config>(data.Content);

            return config;
        }

        public Config GetWithDetails(int id)
        {
            var request = new RestRequest($"configs/{id}/details", Method.GET);
            var data = Execute(request);
            var config = JsonConvert.DeserializeObject<Config>(data.Content);

            return config;
        }

        public Config GetActiveDetails()
        {
            var request = new RestRequest("configs/active/details", Method.GET);
            var data = Execute(request);
            var config = JsonConvert.DeserializeObject<Config>(data.Content);

            return config;
        }

        public IEnumerable<ConfigDetail> GetDetails()
        {
            var request = new RestRequest("configdetails", Method.GET);
            var data = Execute(request);
            var configDetails = JsonConvert.DeserializeObject<ConfigDetailList>(data.Content).ConfigDetails;

            return configDetails;
        }

        public ConfigDetail GetDetail(int detailId)
        {
            var request = new RestRequest($"configdetails/{detailId}", Method.GET);
            var data = Execute(request);
            var configDetail = JsonConvert.DeserializeObject<ConfigDetail>(data.Content);

            return configDetail;
        }

        public int Activate(int id)
        {
            var request = new RestRequest($"configs/{id}/activate", Method.POST);
            var response = Execute(request);

            var result = Convert.ToInt32(response.Content);
            return result;
        }

        public int Post(ConfigEdit config)
        {
            var request = new RestRequest("configs", Method.POST);
            var json = request.JsonSerializer.Serialize(config);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public int PostDetail(ConfigDetailEdit configDetail)
        {
            var request = new RestRequest("configdetails", Method.POST);
            var json = request.JsonSerializer.Serialize(configDetail);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public void Patch(int id, ConfigEdit config)
        {
            var request = new RestRequest($"configs/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(config);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            Execute(request);
        }

        public void PatchDetail(int detailId, ConfigDetailEdit configDetail)
        {
            var request = new RestRequest($"configdetails/{detailId}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(configDetail);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            Execute(request);
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"configs/{id}", Method.DELETE);
            return Execute(request).Content;
        }

        public string DeleteDetail(int detailId)
        {
            var request = new RestRequest($"configdetails/{detailId}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
