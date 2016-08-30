using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.IO;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/profiledetails")]
    public class ProfileDetailsController : ApiController
    {
        private readonly IProfileDetailService _profileDetailService;

        public ProfileDetailsController(IProfileDetailService profileDetailService)
        {
            _profileDetailService = profileDetailService;
        }

        // GET: api/ProfileDetails
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ProfileDetail> profileDetails = new Collection<ProfileDetail>();

            await Task.Run(() =>
            {
                profileDetails = _profileDetailService.Get();
            });

            if (profileDetails == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ProfileDetails = profileDetails.Select(pd => new
            {
                pd.Id,
                pd.ProfileId,
                pd.ActivityTypeId,
                pd.ResponseTypeId
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ProfileDetails/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var profileDetail = new ProfileDetail();

            await Task.Run(() =>
            {
                profileDetail = _profileDetailService.Get(id);
            });

            if (profileDetail == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = profileDetail.Id;
            exObj.ProfileId = profileDetail.ProfileId;
            exObj.ActivityTypeId = profileDetail.ActivityTypeId;
            exObj.ResponseTypeId = profileDetail.ResponseTypeId;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ProfileDetails/5
        [Route("{id}")]
        [HttpPost]
        public void Post(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var profileDetail = serializer.Deserialize<ProfileDetail>(value);
            profileDetail.ProfileId = id;
            _profileDetailService.Post(profileDetail);
        }

        [Route("{id}/responses")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetResponses(int id)
        {
            var response = new ProfileDetail();

            await Task.Run(() =>
            {
                response = _profileDetailService.GetDetails(id);
            });

            if (response == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = response.Id;
            exObj.ResponseTypeId = response.ResponseTypeId;
            exObj.Responses = response.Responses.Select(rd => new
            {
                rd.Id,
                FilePath = Path.Combine(rd.MediaFile.Path, rd.MediaFile.Filename),
                rd.MediaFile.FileType,
                rd.MediaFile.FileSize
            });

            return new DynamicJsonObject(exObj);
        }

        // PATCH: api/ProfileDetails/5
        [Route("{id}")]
        [HttpPatch]
        public void Patch(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ProfileDetails/5
        public void Delete(int id)
        {
        }
    }
}
