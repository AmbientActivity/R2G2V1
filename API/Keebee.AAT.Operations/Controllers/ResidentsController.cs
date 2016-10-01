using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using Keebee.AAT.Shared;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Dynamic;
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
                    x.GameDifficultyLevel,
                    x.DateCreated,
                    x.DateUpdated
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
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.GameDifficultyLevel = resident.GameDifficultyLevel;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;

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
            exObj.FirstName = resident.FirstName;
            exObj.LastName = resident.LastName;
            exObj.Gender = resident.Gender;
            exObj.GameDifficultyLevel = resident.GameDifficultyLevel;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;

            return new DynamicJsonObject(exObj);
        }

        [HttpGet]
        [Route("{id}/media")]
        public async Task<DynamicJsonObject> GetWithdMedia(int id)
        {
            var resident = new Resident();

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
            exObj.GameDifficultyLevel = resident.GameDifficultyLevel;
            exObj.DateCreated = resident.DateCreated;
            exObj.DateUpdated = resident.DateUpdated;
            exObj.MediaFiles = resident.MediaFiles
                .GroupBy(rt => rt.ResponseType)
                .Select(mediaFiles => new { mediaFiles.First().ResponseType, MediaFiles = mediaFiles })
                .Select(mf => new
                {
                    ResponseType = new
                    {
                        mf.ResponseType.Id,
                        mf.ResponseType.Description,
                        ResponseTypeCategory = new
                        {
                            mf.ResponseType.ResponseTypeCategory.Id,
                            mf.ResponseType.ResponseTypeCategory.Description
                        }
                    },
                    Paths = mf.MediaFiles
                        .GroupBy(pt => pt.MediaPathType)
                        .Select(files => new { files.First().MediaPathType, Files = files })
                        .Select(pt => new
                        {
                            MediaPathType = new
                            {
                                pt.MediaPathType.Id,
                                pt.MediaPathType.Description
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize,
                                f.IsPublic
                            })
                        })
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Residents
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(value);
            var residentId = _residentService.Post(resident);

            return residentId;
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
