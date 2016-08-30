using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IProfileDetailService
    {
        IEnumerable<ProfileDetail> Get();
        ProfileDetail Get(int id);
        void Post(ProfileDetail profileDetail);
        void Patch(int id, ProfileDetail profileDetail);
        void Delete(int id);
        ProfileDetail GetDetails(int id);
    }

    public class ProfileDetailService : IProfileDetailService
    {
        public IEnumerable<ProfileDetail> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profileDetails = container.ProfileDetails.AsEnumerable();

            return profileDetails;
        }

        public ProfileDetail Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profileDetail = container.ProfileDetails.ByKey(id)
                .GetValue();

            return profileDetail;
        }

        public void Post(ProfileDetail profileDetail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToProfileDetails(profileDetail);
            container.SaveChanges();
        }

        public void Patch(int id, ProfileDetail profileDetail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var p = container.ProfileDetails.Where(e => e.Id == id).SingleOrDefault();
            if (p == null) return;

            //if (profileDetail. != null)
            //    p.Description = profileDetail.Description;

            container.UpdateObject(p);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profileDetail = container.ProfileDetails.Where(e => e.Id == id).SingleOrDefault();
            if (profileDetail == null) return;

            container.DeleteObject(profileDetail);
            container.SaveChanges();
        }

        public ProfileDetail GetDetails(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profileDetail = container.ProfileDetails.ByKey(id)
                .Expand("Responses($expand=MediaFile)")
                .GetValue();

            return profileDetail;
        }

        public ResponseType GetResponse(int id, int activityTypeId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var profile = container.Residents.ByKey(id)
                .Profile
                .Expand("ProfileDetails($expand=ActivityType,Response($expand=ResponseType,Responses))")
                .GetValue();

            return profile.ProfileDetails
                .Single(x => x.ActivityTypeId == activityTypeId)
                .ResponseType;
        }
    }
}