using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Net;
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
        string Activate(int id);
        IEnumerable<ConfigDetail> GetDetails();
        ConfigDetail GetDetail(int detailId);
        string Post(ConfigEdit config, out int newId);
        string PostDetail(ConfigDetailEdit configDetail, out int newId);
        string Patch(int id, ConfigEdit config);
        string PatchDetail(int detailId, ConfigDetailEdit configDetail);
        string Delete(int id);
        string DeleteDetail(int detailId);
    }

    public class ConfigsClient : BaseClient, IConfigsClient
    {
        public IEnumerable<Config> Get()
        {
            var request = new RestRequest("configs", Method.GET);
            var data = Execute(request);
            var configs = JsonConvert.DeserializeObject<IEnumerable<Config>>(data.Content);

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
            var configDetails = JsonConvert.DeserializeObject<IEnumerable<ConfigDetail>>(data.Content);

            return configDetails;
        }

        public ConfigDetail GetDetail(int detailId)
        {
            var request = new RestRequest($"configdetails/{detailId}", Method.GET);
            var data = Execute(request);
            var configDetail = JsonConvert.DeserializeObject<ConfigDetail>(data.Content);

            return configDetail;
        }

        public string Activate(int id)
        {
            var request = new RestRequest($"configs/{id}/activate", Method.GET);
            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Post(ConfigEdit config, out int newId)
        {
            var request = new RestRequest("configs", Method.POST);
            var json = request.JsonSerializer.Serialize(config);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            string result = null;
            newId = -1;

            if (response.StatusCode == HttpStatusCode.OK)
                newId = Convert.ToInt32(response.Content);
            else
                result = response.StatusDescription;

            return result;
        }

        public string PostDetail(ConfigDetailEdit configDetail, out int newId)
        {
            var request = new RestRequest("configdetails", Method.POST);
            var json = request.JsonSerializer.Serialize(configDetail);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            string result = null;
            newId = -1;

            if (response.StatusCode == HttpStatusCode.OK)
                newId = Convert.ToInt32(response.Content);
            else
                result = response.StatusDescription;

            return result;
        }

        public string Patch(int id, ConfigEdit config)
        {
            var request = new RestRequest($"configs/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(config);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string PatchDetail(int detailId, ConfigDetailEdit configDetail)
        {
            var request = new RestRequest($"configdetails/{detailId}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(configDetail);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
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
