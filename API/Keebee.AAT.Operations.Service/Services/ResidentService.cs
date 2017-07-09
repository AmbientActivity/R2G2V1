using Keebee.AAT.Operations.Service.KeebeeAAT;
using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IResidentService
    {
        IEnumerable<Resident> Get();
        Resident Get(int id);
        Resident GetByNameGender(string firstName, string lastName, string gender);
        Resident GetWithMedia(int id);
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

            var resident = container.Residents.ByKey(id);

            Resident result;
            try { result = resident.GetValue(); }
            catch { result = null; }

            return result;
        }

        public Resident GetByNameGender(string firstName, string lastName, string gender)
        {
            var container = new Container(new Uri(ODataHost.Url));
            var lastname = lastName == null ? "null" : $"'{lastName}'";

            var residents = container.Residents
                .AddQueryOption("$filter", $"FirstName eq '{firstName}' and LastName eq {lastname} and Gender eq '{gender}'")
                .Expand("MediaFiles($expand=MediaFile)")
                .ToList();

            return residents.Any() ? residents.Single() : null;
        }

        public Resident GetWithMedia(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var resident = container.Residents.ByKey(id)
                .Expand("MediaFiles($expand=MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory))");

            Resident result;
            try { result = resident.GetValue(); }
            catch { result = null; }

            return result;
        }

        public int Post(Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            resident.DateCreated = DateTime.Now;
            resident.DateUpdated = DateTime.Now;

            if (resident.ProfilePicture == null)
                resident.ProfilePicture = new byte[0];

            container.AddToResidents(resident);
            container.SaveChanges();

            return resident.Id;
        }

        public void Patch(int id, Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var r = container.Residents.Where(e => e.Id == id).SingleOrDefault();
            if (r == null) return;

            if (resident.FirstName != null)
                r.FirstName = resident.FirstName;

            r.LastName = resident.LastName;

            if (resident.Gender != null)
                r.Gender = resident.Gender;

            if (resident.GameDifficultyLevel > 0)
                r.GameDifficultyLevel = resident.GameDifficultyLevel;

            r.AllowVideoCapturing = resident.AllowVideoCapturing;
            r.ProfilePicture = resident.ProfilePicture ?? new byte[0];

            r.DateUpdated = DateTime.Now;

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
