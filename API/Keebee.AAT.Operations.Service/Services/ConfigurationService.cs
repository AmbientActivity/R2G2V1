using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IConfigurationService
    {
        IEnumerable<Configuration> Get();
        Configuration Get(int id);
        Configuration GetActiveDetails();
        int Post(Configuration configuration);
        void Patch(int id, Configuration configuration);
        void Delete(int id);
        Configuration GetDetails(int id);
        Configuration GetMediaForProfile(int profileId);
        Configuration GetMediaForProfileActivityResponseType(int profileId, int activityTypeId, int responseTypeId);
        Configuration GetMedia();
    }

    public class ConfigurationService : IConfigurationService
    {
        public IEnumerable<Configuration> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configurations = container.Configurations
                .AsEnumerable();

            return configurations;
        }

        public Configuration Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations.ByKey(id)
                .Expand("ConfigurationDetails")
                .GetValue();

            return configuration;
        }

        public Configuration GetActiveDetails()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations
                .Expand("ConfigurationDetails")
                .AddQueryOption("$filter", "IsActive")
                .Expand("ConfigurationDetails($expand=ActivityType,ResponseType($expand=ResponseTypeCategory))")
                .Single();

            return configuration;
        }

        public int Post(Configuration configuration)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToConfigurations(configuration);
            container.SaveChanges();

            var configurationId = configuration.Id;

            //var responseTypes

            return configurationId;
        }

        public void Patch(int id, Configuration configuration)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var c = container.Configurations.Where(e => e.Id == id).SingleOrDefault();
            if (c == null) return;

            if (configuration.Description != null)
                c.Description = configuration.Description;

            container.UpdateObject(c);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations.Where(e => e.Id == id).SingleOrDefault();
            if (configuration == null) return;

            container.DeleteObject(configuration);
            container.SaveChanges();
        }
        
        public Configuration GetDetails(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations.ByKey(id)
                 .Expand("ConfigurationDetails($expand=ActivityType,ResponseType($expand=ResponseTypeCategory))")
                .GetValue();

            return configuration;
        }

        public Configuration GetMediaForProfile(int profileId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations
                    .AddQueryOption("$filter", "IsActive")
                    .Expand($"ConfigurationDetails($expand=ActivityType,ResponseType($expand=ResponseTypeCategory,Responses($filter=ProfileId eq {profileId};$expand=MediaFile)))")
                    .Single();

            return configuration;
        }

        public Configuration GetMediaForProfileActivityResponseType(int profileId, int activityTypeId, int responseTypeId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations
                    .AddQueryOption("$filter", "IsActive")
                    .Expand($"ConfigurationDetails($filter=ActivityTypeId eq {activityTypeId} and ResponseTypeId eq {responseTypeId};" +
                            $"$expand=ActivityType,ResponseType($expand=ResponseTypeCategory,Responses($filter=ProfileId eq {profileId};$expand=MediaFile)))")
                    .FirstOrDefault();

            return configuration;
        }

        public Configuration GetMedia()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configuration = container.Configurations
                    .AddQueryOption("$filter", "IsActive")
                    .Expand("ConfigurationDetails($expand=ActivityType,ResponseType($expand=Responses($expand=MediaFile)))")
                    .FirstOrDefault();

            return configuration;
        }
    }
}