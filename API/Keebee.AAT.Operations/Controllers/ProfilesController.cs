using System;
using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/profiles")]
    public class ProfilesController : ApiController
    {
        private readonly IProfileService _profileService;
        private readonly IConfigService _configService;

        public ProfilesController(IProfileService profileService, IConfigService configService)
        {
            _profileService = profileService;
            _configService = configService;
        }

        // GET: api/Profiles
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<Profile> profiles = new Collection<Profile>();

            await Task.Run(() =>
            {
                profiles = _profileService.Get();
            });

            if (profiles == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Profiles = profiles.Select(p => new
                                                  {
                                                      p.Id,
                                                      p.Description,
                                                      p.GameDifficultyLevel,
                                                      p.DateCreated,
                                                      p.DateUpdated
                                                  });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Profiles/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _profileService.Get(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.DateUpdated = profile.DateUpdated;

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetProfileDetails(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _profileService.Get(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            var config = _configService.GetMediaForProfile(profile.Id);

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.ConfigDetails = config.ConfigDetails.Select(cd => new
            {
                cd.Id,
                cd.ActivityTypeId,
                ResponseType = new
                {
                    cd.ResponseType.Id,
                    cd.ResponseType.Description,
                    cd.ResponseType.IsInteractive
                }
            });

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/media")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetWithMedia(int id)
        {
            Profile profile = new Profile();

            await Task.Run(() =>
            {
                profile = _profileService.Get(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            var config = _configService.GetMediaForProfile(profile.Id);

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.ConfigDetails = config.ConfigDetails
                .Select(detail => new
                {
                    detail.Id,
                    detail.ConfigId,
                    ActivityType = new
                    {
                        detail.ActivityType.Id,
                        detail.ActivityType.Description,
                        detail.ActivityType.PhidgetType
                    },
                    ResponseType = new
                    {
                        detail.ResponseType.Id,
                        detail.ResponseType.Description,
                        detail.ResponseType.IsInteractive,
                        Responses = detail.ResponseType.Responses
                        .Where(x => x.ProfileId == profile.Id)
                        .Select(response => new
                        {
                            response.Id,
                            response.StreamId,
                            Filename = response.MediaFile.Filename.Replace($".{response.MediaFile.FileType}", string.Empty),
                            FilePath = Path.Combine(response.MediaFile.Path, response.MediaFile.Filename),
                            response.MediaFile.FileType,
                            response.MediaFile.FileSize
                        })
                    },
                    
                });

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/media")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetWithMediaByActivityResponseType(int id, int activityTypeId, int responseTypeId)
        {
            Profile profile = new Profile();

            await Task.Run(() =>
            {
                profile = _profileService.Get(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            var config = _configService.GetMediaForProfileActivityResponseType(profile.Id, activityTypeId, responseTypeId);

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.ConfigDetails = config.ConfigDetails
                .Select(detail => new
                {
                    detail.Id,
                    detail.ConfigId,
                    ActivityType = new
                    {
                        detail.ActivityType.Id,
                        detail.ActivityType.Description,
                        detail.ActivityType.PhidgetType
                    },
                    ResponseType = new
                    {
                        detail.ResponseType.Id,
                        detail.ResponseType.Description,
                        detail.ResponseType.IsInteractive,
                        Responses = detail.ResponseType.Responses
                        .Where(x => x.ProfileId == profile.Id)
                        .Select(response => new
                        {
                            response.Id,
                            response.StreamId,
                            Filename = response.MediaFile.Filename.Replace($".{response.MediaFile.FileType}", string.Empty),
                            FilePath = Path.Combine(response.MediaFile.Path, response.MediaFile.Filename),
                            response.MediaFile.FileType,
                            response.MediaFile.FileSize
                        })
                    },

                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Profiles
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(value);
            profile.DateCreated = DateTime.Now;

            return _profileService.Post(profile);
        }

        // PUT: api/Profiles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(value);
            _profileService.Patch(id, profile);
        }

        // DELETE: api/Profiles/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _profileService.Delete(id);
        }
    }
}
