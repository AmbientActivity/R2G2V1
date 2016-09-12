using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IConfigService
    {
        IEnumerable<Config> Get();
        Config Get(int id);
        Config GetByDescription(string description);
        Config GetActiveDetails();
        int Post(Config config);
        void Patch(int id, Config config);
        void Delete(int id);
        Config GetDetails(int id);
        Config GetMediaForProfile(int profileId);
        Config GetMediaForProfileConfigDetail(int profileId, int configDetailId);
        Config GetMedia();
        void Activate(int id);
    }

    public class ConfigService : IConfigService
    {
        public IEnumerable<Config> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configs = container.Configs
                .Expand("ConfigDetails($expand=PhidgetType,ResponseType)")
                .AsEnumerable();

            return configs;
        }

        public Config Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs
                .ByKey(id)
                .Expand("ConfigDetails($expand=PhidgetType,ResponseType)")
                .GetValue();

            return config;
        }

        public Config GetByDescription(string description)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs
                .AddQueryOption("$filter", $"Description eq '{description}'")
                .Single();

            return config;
        }

        public Config GetActiveDetails()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs
                .AddQueryOption("$filter", "IsActive")
                .Expand("ConfigDetails($expand=PhidgetType,ResponseType($expand=ResponseTypeCategory))")
                .Single();

            return config;
        }

        public int Post(Config config)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToConfigs(config);
            container.SaveChanges();

            var configId = config.Id;

            return configId;
        }

        public void Patch(int id, Config config)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var c = container.Configs.Where(e => e.Id == id).SingleOrDefault();
            if (c == null) return;

            if (config.Description != null)
                c.Description = config.Description;

            container.UpdateObject(c);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs.Where(e => e.Id == id).SingleOrDefault();
            if (config == null) return;

            container.DeleteObject(config);
            container.SaveChanges();
        }
        
        public Config GetDetails(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs.ByKey(id)
                 .Expand("ConfigDetails($expand=PhidgetType,ResponseType($expand=ResponseTypeCategory))")
                .GetValue();

            return config;
        }

        public Config GetMediaForProfile(int profileId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs
                    .AddQueryOption("$filter", "IsActive")
                    .Expand("ConfigDetails($filter=ResponseType/IsSystem eq false;" +
                            "$expand=PhidgetType,ResponseType($expand=ResponseTypeCategory," +
                            $"Responses($filter=ProfileId eq {profileId};$expand=MediaFile)))")
                    .Single();

            return config;
        }

        public Config GetMediaForProfileConfigDetail(int profileId, int configDetailId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs
                    .AddQueryOption("$filter", "IsActive")
                    .Expand($"ConfigDetails($filter=Id eq {configDetailId};" +
                            $"$expand=PhidgetType,ResponseType($filter=IsSystem;$expand=ResponseTypeCategory,Responses($filter=ProfileId eq {profileId};$expand=MediaFile)))")
                    .FirstOrDefault();

            return config;
        }

        public Config GetMedia()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var config = container.Configs
                    .AddQueryOption("$filter", "IsActive")
                    .Expand("ConfigDetails($expand=PhidgetType,ResponseType($expand=Responses($expand=MediaFile)))")
                    .FirstOrDefault();

            return config;
        }

        public void Activate(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configs = container.Configs.ToList();

            if (configs.All(c => c.Id != id)) return;

            foreach (var cg in configs)
            {
                cg.IsActive = (cg.Id == id);
                container.UpdateObject(cg);
                container.SaveChanges();
            }
        }
    }
}