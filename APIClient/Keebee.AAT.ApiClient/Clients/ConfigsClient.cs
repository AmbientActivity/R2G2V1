using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

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
        void Delete(int id);
        void DeleteDetail(int detailId);
    }

    public class ConfigsClient : IConfigsClient
    {
        private readonly ClientBase _clientBase;

        public ConfigsClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<Config> Get()
        {
            var data = _clientBase.Get("configs");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var configs = serializer.Deserialize<ConfigList>(data).Configs;

            return configs;
        }

        public Config Get(int id)
        {
            var data = _clientBase.Get($"configs/{id}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Config GetActive()
        {
            var data = _clientBase.Get("configs/active");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Config GetByDescription(string description)
        {
            var data = _clientBase.Get($"configs?description={description}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Config GetWithDetails(int id)
        {
            var data = _clientBase.Get($"configs/{id}/details");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Config GetActiveDetails()
        {
            var data = _clientBase.Get("configs/active/details");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public IEnumerable<ConfigDetail> GetDetails()
        {
            var data = _clientBase.Get("configdetails");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var configDetails = serializer.Deserialize<ConfigDetailList>(data).ConfigDetails;

            return configDetails;
        }

        public ConfigDetail GetDetail(int detailId)
        {
            var data = _clientBase.Get($"configdetails/{detailId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var configDetail = serializer.Deserialize<ConfigDetail>(data);

            return configDetail;
        }

        public int Activate(int id)
        {
            return _clientBase.Post($"configs/{id}/activate", string.Empty);
        }

        public int Post(ConfigEdit config)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(config);

            return _clientBase.Post("configs", el);
        }

        public int PostDetail(ConfigDetailEdit configDetail)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(configDetail);

            return _clientBase.Post("configdetails", el);
        }

        public void Patch(int id, ConfigEdit config)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(config);

            _clientBase.Patch($"configs/{id}", el);
        }

        public void PatchDetail(int detailId, ConfigDetailEdit configDetail)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(configDetail);

            _clientBase.Patch($"configdetails/{detailId}", el);
        }

        public void Delete(int id)
        {
            _clientBase.Delete($"configs/{id}");
        }

        public void DeleteDetail(int detailId)
        {
            _clientBase.Delete($"configdetails/{detailId}");
        }
    }
}
