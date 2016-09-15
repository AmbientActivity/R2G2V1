using Keebee.AAT.Operations.Service.KeebeeAAT;
using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Keebee.AAT.Operations.Service.FileManagement;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IResidentService
    {
        IEnumerable<Resident> Get();
        Resident Get(int id);
        Resident GetByNameGender(string firstName, string lastName, string gender);
        int Post(Resident resident);
        void Patch(int id, Resident resident);
        void Delete(int id);
        bool ResidentExists(int id);
    }

    public class ResidentService : IResidentService
    {
        public IEnumerable<Resident> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents
                .ToList();
        }

        public Resident Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents.ByKey(id).GetValue();
        }

        public Resident GetByNameGender(string firstName, string lastName, string gender)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents
                .AddQueryOption("$filter", $"FirstName eq '{firstName}' and LastName eq '{lastName}' and Gender eq '{gender}'")
                .Single();
        }

        public int Post(Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            resident.DateCreated = DateTime.Now;
            resident.DateUpdated = DateTime.Now;

            container.AddToResidents(resident);
            container.SaveChanges();

            var fileManager = new FileManager();
            fileManager.CreateFolders(resident.Id);

            return resident.Id;
        }

        public void Patch(int id, Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var r = container.Residents.Where(e => e.Id == id).SingleOrDefault();
            if (r == null) return;

            if (resident.FirstName != null)
                r.FirstName = resident.FirstName;

            if (resident.LastName != null)
                r.LastName = resident.LastName;

            if (resident.Gender != null)
                r.Gender = resident.Gender;

            if (resident.GameDifficultyLevel > 0)
                r.GameDifficultyLevel = resident.GameDifficultyLevel;

            resident.DateUpdated = DateTime.Now;

            container.UpdateObject(r);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var resident = container.Residents.Where(e => e.Id == id).SingleOrDefault();
            if (resident == null) return;

            container.DeleteObject(resident);
            container.SaveChanges();

            var fileManager = new FileManager();
            fileManager.DeleteFolders(id);
        }

        public bool ResidentExists(int id)
        {
            try
            {
                if (id == 0) return false;

                var container = new Container(new Uri(ODataHost.Url));

                var resident = container.Residents.ByKey(id).GetValue();

                return resident != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
