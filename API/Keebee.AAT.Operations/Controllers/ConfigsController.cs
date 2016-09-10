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
    [RoutePrefix("api/configs")]
    public class ConfigsController : ApiController
    {
        private readonly IConfigService _configService;

        public ConfigsController(IConfigService configService)
        {
            _configService = configService;
        }

        // GET: api/Configs
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<Config> configs = new Collection<Config>();

            await Task.Run(() =>
            {
                configs = _configService.Get();
            });

            if (configs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Configs = configs.Select(c => new
            {
                c.Id,
                c.Description,
                c.IsActive,
                ConfigDetails = c.ConfigDetails.Select(cd => new
                    {
                        cd.Id,
                        cd.ConfigId,
                        cd.Description,
                        PhidgetType = new
                        {
                            cd.PhidgetType.Id,
                            cd.PhidgetType.Description,
                        },
                        ResponseType = new
                        {
                            cd.ResponseType.Id,
                            cd.ResponseType.Description,
                            cd.ResponseType.IsInteractive,
                            cd.ResponseType.IsSystem
                        }
                    })
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var config = new Config();

            await Task.Run(() =>
            {
                config = _configService.Get(id);
            });

            if (config == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = config.Id;
            exObj.Description = config.Description;
            exObj.IsActive = config.IsActive;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs/active
        [Route("active/details")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetActiveDetails()
        {
            var config = new Config();

            await Task.Run(() =>
            {
                config = _configService.GetActiveDetails();
            });

            if (config == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = config.Id;
            exObj.Description = config.Description;
            exObj.IsActive = config.IsActive;
            exObj.ConfigDetails = config.ConfigDetails.Select(cd => new
            {
                cd.Id,
                ConfigId = config.Id,
                PhidgetType = new
                {
                    cd.PhidgetType.Id,
                    cd.PhidgetType.Description
                },
                cd.Description,
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
            var config = new Config();

            await Task.Run(() =>
            {
                config = _configService.GetDetails(id);
            });

            if (config == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = config.Id;
            exObj.Description = config.Description;
            exObj.IsActive = config.IsActive;
            exObj.ConfigDetails = config.ConfigDetails.Select(cd => new
            {
                cd.Id,
                ConfigId = config.Id,
                PhidgetType = new
                {
                    cd.PhidgetType.Id,
                    cd.PhidgetType.Description,
                },
                cd.Description,
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
            var config = new Config();

            await Task.Run(() =>
            {
                config = _configService.GetMediaForProfile(id);
            });

            if (config == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = config.Id;
            exObj.Description = config.Description;
            exObj.IsActive = config.IsActive;
            exObj.ConfigDetails = config.ConfigDetails.Select(cd => new
            {
                cd.Id,
                PhidgetType = new
                {
                    cd.PhidgetType.Id,
                    cd.PhidgetType.Description
                },
                cd.Description,
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

        // POST: api/Configs
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(value);

            return _configService.Post(config);
        }

        // POST: api/Configs/2/activate
        [Route("{id}/activate")]
        [HttpPost]
        public void Activate(int id)
        {
            _configService.Activate(id);
        }

        // PUT: api/Configs/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(value);
            _configService.Patch(id, config);
        }

        // DELETE: api/Configs/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _configService.Delete(id);
        }
    }
}
