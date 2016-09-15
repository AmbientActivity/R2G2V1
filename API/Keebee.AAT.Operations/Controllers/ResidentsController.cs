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
        private readonly IConfigService _configurationService;

        public ResidentsController(IResidentService residentService, IConfigService configurationService)
        {
            _residentService = residentService;
            _configurationService = configurationService;
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
                    x.FirstName,
                    x.LastName,
                    x.Gender,
                    x.DateCreated,
                    x.DateUpdated,
                    Profile = new
                    {
                        x.Profile.Id,
                        x.Profile.Description,
                        x.Profile.GameDifficultyLevel,
                        x.Profile.DateCreated,
                        x.Profile.DateUpdated,
                    }
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
            exObj.ProfileId = resident.ProfileId;
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;
            exObj.Profile = new
                {
                    resident.Profile.Id,
                    resident.Profile.Description,
                    resident.Profile.GameDifficultyLevel,
                    resident.Profile.DateCreated,
                    resident.Profile.DateUpdated
            };
            return new DynamicJsonObject(exObj);
        }

        [HttpGet]
        public async Task<DynamicJsonObject> GetNyNameGender(string firstName, string lastName, string gender)
        {
            var resident = new Resident();

            await Task.Run(() =>
            {
                resident = _residentService.GetByNameGender(firstName, lastName, gender);
            });

            if (resident == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = resident.Id;
            exObj.ProfileId = resident.ProfileId;
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;
            return new DynamicJsonObject(exObj);
        }

        [Route("details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetDetails()
        {
            IEnumerable<Resident> residents = new Collection<Resident>();

            await Task.Run(() =>
            {
                residents = _residentService.Get();
            });

            if (residents == null) return new DynamicJsonObject(new ExpandoObject());

            var configuration = _configurationService.GetActiveDetails();

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
                        ConfigDetails = configuration
                        .ConfigDetails.Select(detail => new
                        {
                            ResponseType = new
                            {
                                detail.ResponseType.Id,
                                detail.ResponseType.Description,
                                detail.ResponseType.IsInteractive
                            }
                        })
                    }
        });

            return new DynamicJsonObject(exObj);
        }

        [HttpGet]
        [Route("{id}/details")]
        public async Task<DynamicJsonObject> GetDetails(int id)
        {
            var resident = new Resident();

            await Task.Run(() =>
            {
                resident = _residentService.Get(id);
            });

            if (resident == null) return new DynamicJsonObject(new ExpandoObject());
            var configuration = _configurationService.GetActiveDetails();

            dynamic exObj = new ExpandoObject();
            exObj.Id = resident.Id;
            exObj.ProfileId = resident.ProfileId;
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;
            exObj.Profile = new
            {
                resident.Profile.Id,
                resident.Profile.Description,
                resident.Profile.GameDifficultyLevel,
                resident.Profile.DateCreated,
                resident.Profile.DateUpdated,
                ConfigDetails = configuration
                        .ConfigDetails.Select(detail => new
                        {
                            ResponseType = new
                            {
                                detail.ResponseType.Id,
                                detail.ResponseType.Description,
                                detail.ResponseType.IsInteractive
                            }
                        })
            };
            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/profile")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetProfile(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _residentService.Get(id).Profile;
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.ResidentId = id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.DateUpdated = profile.DateUpdated;

            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/profile/details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetProfileDetails(int id)
        {
            var profile = new Profile();

            await Task.Run(() =>
            {
                profile = _residentService.Get(id).Profile;
            });

            if (profile == null) return new DynamicJsonObject(new ExpandoObject());

            var configuration = _configurationService.GetDetailsForProfile(profile.Id);

            dynamic exObj = new ExpandoObject();
            exObj.Id = profile.Id;
            exObj.Description = profile.Description;
            exObj.GameDifficultyLevel = profile.GameDifficultyLevel;
            exObj.DateCreated = profile.DateCreated;
            exObj.DateUpdated = profile.DateUpdated;
            exObj.ConfigDetails = configuration.ConfigDetails.Select(c => new 
            {
                c.Id,
                c.ConfigId,
                PhidgetType = new
                {
                    c.PhidgetType.Id,
                    c.PhidgetType.Description
                },
                ResponseType = new
                {
                    c.ResponseType.Id,
                    c.ResponseType.Description,
                    c.ResponseType.IsInteractive
                }
            });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Residents
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(value);
            return _residentService.Post(resident);
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
