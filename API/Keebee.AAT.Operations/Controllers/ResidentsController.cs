using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/residents")]
    public class ResidentsController : ApiController
    {
        private readonly IResidentService _residentService;

        public ResidentsController(IResidentService residentService)
        {
            _residentService = residentService;
        }

        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<Resident> residents = new Collection<Resident>();

            await Task.Run(() =>
            {
                residents = _residentService.Get();
            });

            if (residents == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Residents = residents
                .Select(x => new
                {
                    x.Id,
                    x.Tag,
                    x.FirstName,
                    x.LastName,
                    x.Gender,
                    x.DateCreated,
                    x.DateUpdated,
                    x.ProfileId
                });

            return new DynamicJsonObject(exObj);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var resident = new Resident();

            await Task.Run(() =>
            {
                resident = _residentService.Get(id);
            });

            if (resident == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = resident.Id;
            exObj.Tag = resident.Tag;
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;
            exObj.ProfileId = resident.ProfileId;

            return new DynamicJsonObject(exObj);
        }

        [Route("media")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetWithMedia()
        {
            IEnumerable<Resident> residents = new Collection<Resident>();

            await Task.Run(() =>
            {
                residents = _residentService.GetWithMedia();
            });

            if (residents == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Residents = residents
                .Select(resident => new
                {
                    resident.Id,
                    resident.FirstName,
                    resident.LastName,
                    Profile = new
                    {
                        Id = resident.ProfileId,
                        ResidentId = resident.Id,
                        resident.Profile.GameDifficultyLevel,
                        ProfileDetails = resident.Profile.ProfileDetails.Select(detail => new
                        {
                            ResponseType = new
                            {
                                detail.ResponseType.Id,
                                detail.ResponseType.Description,
                                detail.ResponseType.IsInteractive
                            },    
                            Responses = detail.Responses.Select(response => new
                            {
                                response.Id,
                                response.StreamId,
                                Filename = response.MediaFile.Filename.Replace($".{response.MediaFile.FileType}", string.Empty),
                                FilePath = Path.Combine(response.MediaFile.Path, response.MediaFile.Filename),
                                response.MediaFile.FileType,
                                response.MediaFile.FileSize
                            })
                        })
                    },
                    PersonalPictures = resident.PersonalPictures.Select(r => new
                    {
                        r.Id,
                        ResidentId = resident.Id,
                        r.StreamId,
                        Filename = r.MediaFile.Filename.Replace($".{r.MediaFile.FileType}", string.Empty),
                        FilePath = Path.Combine(r.MediaFile.Path, r.MediaFile.Filename),
                        r.MediaFile.FileType,
                        r.MediaFile.FileSize
                    })
        });

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/media")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetWithMedia(int id)
        {
            Resident resident = new Resident();

            await Task.Run(() =>
            {
                resident = _residentService.GetWithMedia(id);
            });

            if (resident == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = resident.Id;
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.Tag = resident.Tag;
            exObj.Profile = new
                {
                    Id = resident.ProfileId,
                    ResidentId = resident.Id,
                    resident.Profile.GameDifficultyLevel,
                    ProfileDetails = resident.Profile.ProfileDetails.Select(detail => new
                    {
                        ResponseType = new
                        {
                            detail.ResponseType.Id,
                            detail.ResponseType.Description,
                            detail.ResponseType.IsInteractive
                        },
                        Responses = detail.Responses.Select(response => new
                        {
                            response.Id,
                            response.StreamId,
                            Filename = response.MediaFile.Filename.Replace($".{response.MediaFile.FileType}", string.Empty),
                            FilePath = Path.Combine(response.MediaFile.Path, response.MediaFile.Filename),
                            response.MediaFile.FileType,
                            response.MediaFile.FileSize
                        })
                    })
                };
            exObj.PersonalPictures = resident.PersonalPictures.Select(p => new
                {
                    p.Id,
                    ResidentId = resident.Id,
                    p.StreamId,
                    Filename = p.MediaFile.Filename.Replace($".{p.MediaFile.FileType}", string.Empty),
                    FilePath = Path.Combine(p.MediaFile.Path, p.MediaFile.Filename),
                    p.MediaFile.FileType,
                    p.MediaFile.FileSize
                });

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/profile")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetProfile(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _residentService.GetProfile(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.DateCreated = profile.DateUpdated;

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/profile/details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetProfileDetails(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _residentService.GetProfileDetails(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.DateUpdated = profile.DateUpdated;
            exObj.ProfileDetails = profile.ProfileDetails.Select(x => new 
            {
                x.Id,
                x.ActivityTypeId,
                ResponseType = new
                {
                    x.ResponseType.Id,
                    x.ResponseType.Description,
                    x.ResponseType.IsInteractive
                }
            });

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/profile/activitytypes")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetProfileActivityTypes(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _residentService.GetProfileActivityTypes(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Profile = profile.Description;
            exObj.ActivityTypes = profile.ProfileDetails.Select(x => new
            {
                x.ActivityTypeId,
                x.ActivityType.Description
            });

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/pictures")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetPictures(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _residentService.GetProfile(id);
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.DateCreated = profile.DateUpdated;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Residents
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(value);
            _residentService.Post(resident);
        }

        // PATCH: api/Residents/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(value);
            _residentService.Patch(id, resident);
        }

        // DELETE: api/Residents/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _residentService.Delete(id);
        }
    }
}
