using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IResponseService
    {
        IEnumerable<Response> Get();
        Response Get(int id);
        void Post(Response response);
        void Patch(int id, Response response);
        void Delete(int id);
    }

    public class ResponseService : IResponseService
    {
        public IEnumerable<Response> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responses = container.Responses
                .Expand("MediaFile")
                .AsEnumerable();

            return responses;
        }

        public Response Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var response = container.Responses.ByKey(id)
                .Expand("MediaFile")
                .GetValue();

            return response;
        }

        public void Post(Response response)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToResponses(response);
            container.SaveChanges();
        }

        public void Patch(int id, Response response)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var p = container.Responses.Where(e => e.Id == id).SingleOrDefault();
            if (p == null) return;

            container.UpdateObject(p);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var response = container.Responses.Where(e => e.Id == id).SingleOrDefault();
            if (response == null) return;

            container.DeleteObject(response);
            container.SaveChanges();
        }
    }
}