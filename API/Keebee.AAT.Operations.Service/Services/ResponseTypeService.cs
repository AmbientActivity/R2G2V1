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
        IEnumerable<ResponseType> GetRandomTypes();
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
                .Expand("ResponseTypeCategory,InteractiveActivityType");

            ResponseType result;
            try { result = responseType.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<ResponseType> GetRandomTypes()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responseTypes = container.ResponseTypes
                .AddQueryOption("$filter", "IsRandom eq true")
                .Expand("InteractiveActivityType")
                .AsEnumerable();

            return responseTypes;
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

            el.ResponseTypeCategoryId = responseType.ResponseTypeCategoryId;
            el.InteractiveActivityTypeId = responseType.InteractiveActivityTypeId;
            el.IsSystem = responseType.IsSystem;
            el.IsRandom = responseType.IsRandom;
            
            if (!string.IsNullOrEmpty(el.Description))
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
