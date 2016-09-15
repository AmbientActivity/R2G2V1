using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using Keebee.AAT.Operations.Service.FileManagement;
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
        [Route("generic")]
        public async Task<DynamicJsonObject> GetGeneric()
        {
            var configuration = new Config();

            await Task.Run(() =>
            {
                configuration = _configurationService.GetActiveDetails();
            });

            if (configuration == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = GenericMedia.Id;
            exObj.FirstName = GenericMedia.Description;
            exObj.GameDifficultyLevel = 1;
            exObj.ConfigDetails = configuration.ConfigDetails.Select(detail => new
            {
                PhidgetType = new
                {
                    detail.PhidgetType.Id,
                    detail.PhidgetType.Description,
                    PhidgetStyleType = new
                    {
                        detail.PhidgetStyleType.Id,
                        detail.PhidgetStyleType.Description
                    }
                },
                ResponseType = new
                {
                    detail.ResponseType.Id,
                    detail.ResponseType.Description,
                    detail.ResponseType.IsInteractive,
                    ResponseCategoryType = new
                    {
                        detail.ResponseType.ResponseTypeCategory.Id,
                        detail.ResponseType.ResponseTypeCategory.Description
                    }
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
            var residentId = _residentService.Post(resident);

            var fileManager = new FileManager();
            fileManager.CreateFolders(residentId);

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
