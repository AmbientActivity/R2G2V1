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
        Profile GetWithMedia(int id);
        void Post(Profile profile);
        void Patch(int id, Profile profile);
        void Delete(int id);
        Profile GetDetails(int id);
    }

    public class ProfileService : IProfileService
    {
        public IEnumerable<Profile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profiles = container.Profiles.AsEnumerable();

            return profiles;
        }

        public Profile GetWithMedia(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            return container.Profiles.ByKey(id)
                .Expand("ProfileDetails($expand=ResponseType,Responses($expand=MediaFile))")
                .GetValue();
        }

        public Profile Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Profiles.ByKey(id)
                .GetValue();

            return profile;
        }

        public void Post(Profile profile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToProfiles(profile);
            container.SaveChanges();
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

        public Profile GetDetails(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Profiles.ByKey(id)
                .Expand("ProfileDetails($expand=ResponseType)")
                .GetValue();

            return profile;
        }

        public ResponseType GetResponseType(int id, int activityTypeId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Residents.ByKey(id)
                .Profile
                .Expand("ProfileDetails($expand=ActivityType,ResponseType,Responses)")
                .GetValue();

            return profile.ProfileDetails
                .Single(x => x.ActivityTypeId == activityTypeId)
                .ResponseType;
        }
    }
}