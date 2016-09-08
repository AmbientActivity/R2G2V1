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
    [RoutePrefix("api/configurations")]
    public class ConfigurationsController : ApiController
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationsController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        // GET: api/Configurations
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<Configuration> configurations = new Collection<Configuration>();

            await Task.Run(() =>
            {
                configurations = _configurationService.Get();
            });

            if (configurations == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Configurations = configurations.Select(c => new
            {
                c.Id,
                c.Description
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configurations/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var configuration = new Configuration();

            await Task.Run(() =>
            {
                configuration = _configurationService.Get(id);
            });

            if (configuration == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = configuration.Id;
            exObj.Description = configuration.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configurations/active
        [Route("active/details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetActiveDetails()
        {
            var configuration = new Configuration();

            await Task.Run(() =>
            {
                configuration = _configurationService.GetActiveDetails();
            });

            if (configuration == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = configuration.Id;
            exObj.Description = configuration.Description;
            exObj.IsActive = configuration.IsActive;
            exObj.ConfigurationDetails = configuration.ConfigurationDetails.Select(cd => new
            {
                cd.Id,
                ConfigurationId = configuration.Id,
                ActivityType = new
                {
                    cd.ActivityType.Id,
                    cd.ActivityType.Description
                },
                ResponseType = new
                {
                    ResponseTypeCategory = new
                    {
                        cd.ResponseType.ResponseTypeCategory.Id,
                        cd.ResponseType.ResponseTypeCategory.Description
                    },
                    cd.ResponseType.Id,
                    cd.ResponseType.Description,
                    cd.ResponseType.IsInteractive,
                    cd.ResponseType.IsSystem
                }
            });
            return new DynamicJsonObject(exObj);
        }

        [Route("{id}/details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetDetails(int id)
        {
            var configuration = new Configuration();

            await Task.Run(() =>
            {
                configuration = _configurationService.GetDetails(id);
            });

            if (configuration == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = configuration.Id;
            exObj.Description = configuration.Description;
            exObj.IsActive = configuration.IsActive;
            exObj.ConfigurationDetails = configuration.ConfigurationDetails.Select(cd => new
            {
                cd.Id,
                ConfigurationId = configuration.Id,
                ActivityType = new
                {
                    cd.ActivityType.Id,
                    cd.ActivityType.Description
                },
                ResponseType = new
                {
                    ResponseTypeCategory = new
                    {
                        cd.ResponseType.ResponseTypeCategory.Id,
                        cd.ResponseType.ResponseTypeCategory.Description                      
                    },
                    cd.ResponseType.Id,
                    cd.ResponseType.Description,
                    cd.ResponseType.IsInteractive,
                    cd.ResponseType.IsSystem
                }
            });

            return new DynamicJsonObject(exObj);
        }

        [Route("profiles/{id}/media")]
        [HttpGet]
        public async Task<DynamicJsonObject> GeMediaForProfile(int id)
        {
            var configuration = new Configuration();

            await Task.Run(() =>
            {
                configuration = _configurationService.GetMediaForProfile(id);
            });

            if (configuration == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = configuration.Id;
            exObj.Description = configuration.Description;
            exObj.IsActive = configuration.IsActive;
            exObj.ConfigurationDetails = configuration.ConfigurationDetails.Select(cd => new
            {
                cd.Id,
                ActivityType = new
                {
                    cd.ActivityType.Id,
                    cd.ActivityType.Description
                },
                ResponseType = new
                {
                    cd.ResponseType.Id,
                    cd.ResponseType.Description,
                    cd.ResponseType.IsInteractive,
                    cd.ResponseType.IsSystem,
                    Responses = cd.ResponseType.Responses
                        .Select(response => new
                        {
                            response.Id,
                            response.StreamId,
                            Filename = response.MediaFile.Filename.Replace($".{response.MediaFile.FileType}", string.Empty),
                            FilePath = Path.Combine(response.MediaFile.Path, response.MediaFile.Filename),
                            response.MediaFile.FileType
                        })
                  }
            });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Configurations
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var configuration = serializer.Deserialize<Configuration>(value);

            return _configurationService.Post(configuration);
        }

        // PUT: api/Configurations/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var configuration = serializer.Deserialize<Configuration>(value);
            _configurationService.Patch(id, configuration);
        }

        // DELETE: api/Configurations/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _configurationService.Delete(id);
        }
    }
}
