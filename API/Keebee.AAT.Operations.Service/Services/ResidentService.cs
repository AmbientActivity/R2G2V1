using Keebee.AAT.Operations.Service.KeebeeAAT;
using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IResidentService
    {
        IEnumerable<Resident> Get();
        Resident Get(int id);
        int Post(Resident resident);
        void Patch(int id, Resident resident);
        void Delete(int id);
        //Profile GetProfile(int id);
        bool ProfileExists(int id);
        //Profile GetConfigurationDetails(int id);
        //Profile GetProfileActivityTypes(int id);
        //Profile GetResponses(int id);
    }

    public class ResidentService : IResidentService
    {
        public IEnumerable<Resident> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents.Expand("Profile").ToList();
        }

        public Resident Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents.ByKey(id).Expand("Profile").GetValue();
        }

        public int Post(Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            resident.DateCreated = DateTime.Now;
            resident.DateUpdated = DateTime.Now;
            resident.ProfileId = (resident.ProfileId > 0 ? resident.ProfileId : ProfileId.Generic);

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

            if (resident.LastName != null)
                r.LastName = resident.LastName;

            if (resident.Gender != null)
                r.Gender = resident.Gender;

            if (resident.ProfileId != null)
                r.ProfileId = resident.ProfileId;

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
        }

        public bool ProfileExists(int id)
        {
            try
            {
                if (id == 0) return false;

                var container = new Container(new Uri(ODataHost.Url));

                var profile = container.Residents.ByKey(id).Profile.GetValue();

                return profile != null;
            }
            catch
            {
                return false;
            }
        }

        //public Profile GetConfigurationDetails(int id)
        //{
        //    var container = new Container(new Uri(ODataHost.Url));

        //    var profile = container.Residents.ByKey(id)
        //        .Profile
        //        .Expand("ProfileDetails($expand=ActivityType,ResponseType,Responses)")
        //        .GetValue();

        //    return profile;
        //}

        //public Profile GetProfileActivityTypes(int id)
        //{
        //    var container = new Container(new Uri(ODataHost.Url));

        //    var profile = container.Residents.ByKey(id)
        //        .Profile
        //        .Expand("ProfileDetails($expand=ActivityType)")
        //        .GetValue();

        //    return profile;
        //}

        //public Profile GetResponses(int id)
        //{
        //    var container = new Container(new Uri(ODataHost.Url));

        //    return container.Residents.ByKey(id)
        //        .Profile
        //        .Expand("ProfileDetails($expand=Activity($expand=ActivityType),Response($expand=ResponseType,Responses))")
        //        .GetValue();
        //}
    }
}
