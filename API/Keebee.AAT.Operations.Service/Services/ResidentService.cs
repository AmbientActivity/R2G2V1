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
        IEnumerable<Resident> GetWithMedia();
        Resident GetWithMedia(int id);
        void Post(Resident resident);
        void Patch(int id, Resident resident);
        void Delete(int id);
        Profile GetProfile(int id);
        bool ProfileExists(int id);
        Profile GetProfileDetails(int id);
        Profile GetProfileActivityTypes(int id);
        Profile GetResponses(int id);
    }

    public class ResidentService : IResidentService
    {
        public IEnumerable<Resident> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents;
        }

        public Resident Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents.ByKey(id).GetValue();
        }

        public IEnumerable<Resident> GetWithMedia()
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents
                .Expand("Profile($expand=ProfileDetails($expand=ResponseType,Responses($expand=MediaFile))),PersonalPictures($expand=MediaFile)")
                .ToList();
        }

        public Resident GetWithMedia(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents.ByKey(id)
                .Expand("Profile($expand=ProfileDetails($expand=ResponseType,Responses($expand=MediaFile))),PersonalPictures($expand=MediaFile)")
                .GetValue();
        }

        public void Post(Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            resident.ProfileId = (resident.ProfileId > 0 ? resident.ProfileId : null);

            container.AddToResidents(resident);
            container.SaveChanges();
        }

        public void Patch(int id, Resident resident)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var r = container.Residents.Where(e => e.Id == id).SingleOrDefault();
            if (r == null) return;

            if (resident.Tag != null)
                r.Tag = resident.Tag;

            if (resident.FirstName != null)
                r.FirstName = resident.FirstName;

            if (resident.LastName != null)
                r.LastName = resident.LastName;

            if (resident.Gender != null)
                r.Gender = resident.Gender;

            if (resident.ProfileId != null)
                r.ProfileId = resident.ProfileId;

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

        public Profile GetProfile(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Residents.ByKey(id).Profile.GetValue();

            return profile;
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

        public Profile GetProfileDetails(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Residents.ByKey(id)
                .Profile
                .Expand("ProfileDetails($expand=ActivityType,ResponseType,Responses)")
                .GetValue();

            return profile;
        }

        public Profile GetProfileActivityTypes(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Residents.ByKey(id)
                .Profile
                .Expand("ProfileDetails($expand=ActivityType)")
                .GetValue();

            return profile;
        }

        public Profile GetResponses(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Residents.ByKey(id)
                .Profile
                .Expand("ProfileDetails($expand=Activity($expand=ActivityType),Response($expand=ResponseType,Responses))")
                .GetValue();
        }
    }
}
