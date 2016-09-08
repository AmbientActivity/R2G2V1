using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IProfileService
    {
        IEnumerable<Profile> Get();
        Profile Get(int id);
        int Post(Profile profile);
        void Patch(int id, Profile profile);
        void Delete(int id);
    }

    public class ProfileService : IProfileService
    {
        public IEnumerable<Profile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profiles = container.Profiles.AsEnumerable();

            return profiles;
        }

        public Profile Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Profiles.ByKey(id)
                .GetValue();

            return profile;
        }

        public int Post(Profile profile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            profile.DateCreated = DateTime.Now;
            profile.DateUpdated = DateTime.Now;

            container.AddToProfiles(profile);
            container.SaveChanges();

            var profileId = profile.Id;

            return profileId;
        }

        public void Patch(int id, Profile profile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var p = container.Profiles.Where(e => e.Id == id).SingleOrDefault();
            if (p == null) return;

            if (profile.Description != null)
                p.Description = profile.Description;

            container.UpdateObject(p);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Profiles.Where(e => e.Id == id).SingleOrDefault();
            if (profile == null) return;

            container.DeleteObject(profile);
            container.SaveChanges();
        }
    }
}