using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IResponseTypeService
    {
        IEnumerable<ResponseType> Get();
        ResponseType Get(int id);
        int Post(ResponseType responseType);
        void Patch(int id, ResponseType responseType);
        void Delete(int id);
    }

    public class ResponseTypeService : IResponseTypeService
    {
        public IEnumerable<ResponseType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responseTypes = container.ResponseTypes
                .Expand("ResponseTypeCategory,InteractiveActivityType")
                .AsEnumerable();

            return responseTypes;
        }

        public ResponseType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responseType = container.ResponseTypes.ByKey(id)
                .Expand("ResponseTypeCategory,InteractiveActivityType")
                .GetValue();

            return responseType;
        }

        public int Post(ResponseType responseType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToResponseTypes(responseType);
            container.SaveChanges();

            return responseType.Id;
        }

        public void Patch(int id, ResponseType responseType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ResponseTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResponseTypeCategoryId != null)
                el.ResponseTypeCategoryId = responseType.ResponseTypeCategoryId;

            if (el.Description != null)
                el.Description = responseType.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responseType = container.ResponseTypes.Where(e => e.Id == id).SingleOrDefault();
            if (responseType == null) return;

            container.DeleteObject(responseType);
            container.SaveChanges();
        }
    }
}
