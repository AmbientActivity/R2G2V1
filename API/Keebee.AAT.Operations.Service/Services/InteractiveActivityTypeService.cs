using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IInteractiveActivityTypeService
    {
        IEnumerable<InteractiveActivityType> Get();
        InteractiveActivityType Get(int id);
        int Post(InteractiveActivityType interactiveActivityType);
        void Patch(int id, InteractiveActivityType interactiveActivityType);
        void Delete(int id);
    }

    public class InteractiveActivityTypeService : IInteractiveActivityTypeService
    {
        public IEnumerable<InteractiveActivityType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var interactiveActivityTypes = container.InteractiveActivityTypes
                .AsEnumerable();

            return interactiveActivityTypes;
        }

        public InteractiveActivityType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var interactiveActivityType = container.InteractiveActivityTypes.ByKey(id);

            InteractiveActivityType result;
            try { result = interactiveActivityType.GetValue(); }
            catch { result = null; }

            return result;
        }

        public int Post(InteractiveActivityType interactiveActivityType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToInteractiveActivityTypes(interactiveActivityType);
            container.SaveChanges();

            return interactiveActivityType.Id;
        }

        public void Patch(int id, InteractiveActivityType interactiveActivityType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.InteractiveActivityTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (!string.IsNullOrEmpty(el.SwfFile))
                el.SwfFile = interactiveActivityType.SwfFile;

            if (!string.IsNullOrEmpty(el.Description))
                el.Description = interactiveActivityType.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var interactiveActivityType = container.InteractiveActivityTypes.Where(e => e.Id == id).SingleOrDefault();
            if (interactiveActivityType == null) return;

            container.DeleteObject(interactiveActivityType);
            container.SaveChanges();
        }
    }
}
