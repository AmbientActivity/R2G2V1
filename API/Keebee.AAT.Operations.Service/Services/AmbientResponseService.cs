using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IAmbientResponseService
    {
        IEnumerable<AmbientResponse> Get();
        AmbientResponse Get(int id);
        void Post(AmbientResponse response);
        void Patch(int id, AmbientResponse response);
        void Delete(int id);
    }

    public class AmbientResponseService : IAmbientResponseService
    {
        public IEnumerable<AmbientResponse> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responses = container.AmbientResponses
                .Expand("MediaFile")
                .AsEnumerable();

            return responses;
        }

        public AmbientResponse Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var response = container.AmbientResponses.ByKey(id)
                .Expand("MediaFile")
                .GetValue();

            return response;
        }

        public void Post(AmbientResponse response)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToAmbientResponses(response);
            container.SaveChanges();
        }

        public void Patch(int id, AmbientResponse response)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var p = container.AmbientResponses.Where(e => e.Id == id).SingleOrDefault();
            if (p == null) return;

            if (response.ResponseTypeId != null)
                p.ResponseTypeId = response.ResponseTypeId;

            container.UpdateObject(p);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var response = container.AmbientResponses.Where(e => e.Id == id).SingleOrDefault();
            if (response == null) return;

            container.DeleteObject(response);
            container.SaveChanges();
        }
    }
}