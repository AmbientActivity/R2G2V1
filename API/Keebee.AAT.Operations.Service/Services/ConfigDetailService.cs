using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IConfigDetailService
    {
        IEnumerable<ConfigDetail> Get();
        ConfigDetail Get(int id);
        ConfigDetail GetWithMedia(int id);
        int Post(ConfigDetail configDetail);
        void Patch(int id, ConfigDetail configDetail);
        void Delete(int id);
    }

    public class ConfigDetailService : IConfigDetailService
    {
        public IEnumerable<ConfigDetail> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configDetails = container.ConfigDetails
                .Expand("Config,PhidgetType,ResponseType").AsEnumerable();

            return configDetails;
        }

        public ConfigDetail Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configDetail = container.ConfigDetails.ByKey(id)
                .Expand("Config,PhidgetType,ResponseType")
                .GetValue();

            return configDetail;
        }

        public ConfigDetail GetWithMedia(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.ConfigDetails.ByKey(id)
                .Expand("PhidgetType,ResponseType")
                .GetValue();
        }

        public int Post(ConfigDetail configDetail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToConfigDetails(configDetail);
            container.SaveChanges();

            var configDetailId = configDetail.Id;

            return configDetailId;
        }

        public void Patch(int id, ConfigDetail configDetail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ConfigDetails.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (configDetail.Description != null)
                el.Description = configDetail.Description;

            if (configDetail.PhidgetTypeId > 0)
                el.PhidgetTypeId = configDetail.PhidgetTypeId;

            if (configDetail.ResponseTypeId > 0)
                el.ResponseTypeId = configDetail.ResponseTypeId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var configDetail = container.ConfigDetails.Where(e => e.Id == id).SingleOrDefault();
            if (configDetail == null) return;

            container.DeleteObject(configDetail);
            container.SaveChanges();
        }
    }
}