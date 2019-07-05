using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IPhidgetTypeService
    {
        IEnumerable<PhidgetType> Get();
        PhidgetType Get(int id);
        void Post(PhidgetType phidgetType);
        void Patch(int id, PhidgetType phidgetType);
        void Delete(int id);
    }

    public class PhidgetTypeService : IPhidgetTypeService
    {
        public IEnumerable<PhidgetType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var phidgetTypes = container.PhidgetTypes
                .AsEnumerable();

            return phidgetTypes;
        }

        public PhidgetType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var phidgetType = container.PhidgetTypes.ByKey(id);

            PhidgetType result;
            try { result = phidgetType.GetValue(); }
            catch { result = null; }

            return result;
        }

        public void Post(PhidgetType phidgetType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToPhidgetTypes(phidgetType);
            container.SaveChanges();
        }

        public void Patch(int id, PhidgetType phidgetType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.PhidgetTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var phidgetType = container.PhidgetTypes.Where(e => e.Id == id).SingleOrDefault();
            if (phidgetType == null) return;

            container.DeleteObject(phidgetType);
            container.SaveChanges();
        }
    }
}
