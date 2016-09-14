using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IPhidgetStyleTypeService
    {
        IEnumerable<PhidgetStyleType> Get();
        PhidgetStyleType Get(int id);
        void Post(PhidgetStyleType phidgetStyleType);
        void Patch(int id, PhidgetStyleType phidgetStyleType);
        void Delete(int id);
    }

    public class PhidgetStyleTypeService : IPhidgetStyleTypeService
    {
        public IEnumerable<PhidgetStyleType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var phidgetStyleTypes = container.PhidgetStyleTypes
                .AsEnumerable();

            return phidgetStyleTypes;
        }

        public PhidgetStyleType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var phidgetStyleType = container.PhidgetStyleTypes.ByKey(id)
                .GetValue();

            return phidgetStyleType;
        }

        public void Post(PhidgetStyleType phidgetStyleType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToPhidgetStyleTypes(phidgetStyleType);
            container.SaveChanges();
        }

        public void Patch(int id, PhidgetStyleType phidgetStyleType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.PhidgetStyleTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var phidgetStyleType = container.PhidgetStyleTypes.Where(e => e.Id == id).SingleOrDefault();
            if (phidgetStyleType == null) return;

            container.DeleteObject(phidgetStyleType);
            container.SaveChanges();
        }
    }
}
