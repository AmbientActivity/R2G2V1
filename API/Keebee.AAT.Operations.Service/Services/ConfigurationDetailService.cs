using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IConfigurationDetailService
    {
        IEnumerable<ConfigurationDetail> Get();
        ConfigurationDetail Get(int id);
        ConfigurationDetail GetWithMedia(int id);
        int Post(ConfigurationDetail configurationDetail);
        void Patch(int id, ConfigurationDetail configurationDetail);
        void Delete(int id);
    }

    public class ConfigurationDetailService : IConfigurationDetailService
    {
        public IEnumerable<ConfigurationDetail> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configurationDetails = container.ConfigurationDetails
                .Expand("ActivityType,ResponseType").AsEnumerable();

            return configurationDetails;
        }

        public ConfigurationDetail GetWithMedia(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.ConfigurationDetails.ByKey(id)
                .Expand("ActivityType,ResponseType")
                .GetValue();
        }

        public ConfigurationDetail Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configurationDetail = container.ConfigurationDetails.ByKey(id)
                .Expand("ActivityType,ResponseType")
                .GetValue();

            return configurationDetail;
        }

        public int Post(ConfigurationDetail configurationDetail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToConfigurationDetails(configurationDetail);
            container.SaveChanges();

            var configurationDetailId = configurationDetail.Id;

            return configurationDetailId;
        }

        public void Patch(int id, ConfigurationDetail configurationDetail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var cd = container.ConfigurationDetails.Where(e => e.Id == id).SingleOrDefault();
            if (cd == null) return;

            if (configurationDetail.ResponseTypeId != null)
                cd.ResponseTypeId = configurationDetail.ResponseTypeId;

            container.UpdateObject(cd);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configurationDetail = container.ConfigurationDetails.Where(e => e.Id == id).SingleOrDefault();
            if (configurationDetail == null) return;

            container.DeleteObject(configurationDetail);
            container.SaveChanges();
        }
    }
}