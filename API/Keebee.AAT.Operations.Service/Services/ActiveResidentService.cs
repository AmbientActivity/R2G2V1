using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IActiveResidentService
    {
        IEnumerable<ActiveResident> Get();
        ActiveResident Get(int id);
        int Post(ActiveResident activeResident);
        void Patch(int id, ActiveResident activeResident);
        void Delete(int id);
    }

    public class ActiveResidentService : IActiveResidentService
    {
        public IEnumerable<ActiveResident> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidents = container.ActiveResidents
                .Expand("Resident")
                .AsEnumerable();

            return activeResidents;
        }

        public ActiveResident Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResident = container.ActiveResidents.ByKey(id)
                .Expand("Resident")
                .GetValue();

            return activeResident;
        }

        public int Post(ActiveResident activeResident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToActiveResidents(activeResident);
            container.SaveChanges();

            return activeResident.Id;
        }

        public void Patch(int id, ActiveResident activeResident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ActiveResidents.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            el.ResidentId = activeResident.ResidentId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResident = container.ActiveResidents.Where(e => e.Id == id).SingleOrDefault();
            if (activeResident == null) return;

            container.DeleteObject(activeResident);
            container.SaveChanges();
        }
    }
}
