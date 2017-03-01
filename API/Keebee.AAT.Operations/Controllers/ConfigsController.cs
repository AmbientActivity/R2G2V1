using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/configs")]
    public class ConfigsController : ApiController
    {
        private readonly IConfigService _configService;
        private readonly IActivityEventLogService _activityEventLogService;

        public ConfigsController(IConfigService configService, IActivityEventLogService activityEventLogService)
        {
            _configService = configService;
            _activityEventLogService = activityEventLogService;
        }

        // GET: api/Configs
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<Config> configs = new Collection<Config>();
            var activityEventLogs = _activityEventLogService.GetWithConfigDetail().ToArray();

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
                c.IsActiveEventLog,
                IsEventLogs = activityEventLogs.Any(x => x.ConfigDetail.ConfigId == c.Id),
                ConfigDetails = c.ConfigDetails.Select(cd => new
                    {
                        cd.Id,
                        cd.ConfigId,
                        cd.Description,
                        cd.Location,
                        IsEventLogs = activityEventLogs.Any(x => x.ConfigDetail.Id == cd.Id),
                        PhidgetType = new
                        {
                            cd.PhidgetType.Id,
                            cd.PhidgetType.Description,
                        },
                        PhidgetStyleType = new
                        {
                            cd.PhidgetStyleType.Id,
                            cd.PhidgetStyleType.Description,
                        },
                        ResponseType = new
                        {
                            cd.ResponseType.Id,
                            cd.ResponseType.Description,
                            InteractiveActivityType = (cd.ResponseType.InteractiveActivityTypeId != null)
                            ? new
                            {
                                cd.ResponseType.InteractiveActivityType.Id,
                                cd.ResponseType.InteractiveActivityType.Description
                            } : null,
                            cd.ResponseType.IsSystem
                        }
                    }).OrderBy(o => o.PhidgetType.Id)
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
            exObj.IsActiveEventLog = config.IsActiveEventLog;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs?description='config name'
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string description)
        {
            var config = new Config();

            await Task.Run(() =>
            {
                config = _configService.GetByDescription(description);
            });

            if (config == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = config.Id;
            exObj.Description = config.Description;
            exObj.IsActive = config.IsActive;
            exObj.IsActiveEventLog = config.IsActiveEventLog;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs/active
        [Route("active")]
        [HttpGet]
        public async Task<DynamicJsonObject> GetActive()
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
            exObj.IsActiveEventLog = config.IsActiveEventLog;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs/active/details
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
            exObj.IsActiveEventLog = config.IsActiveEventLog;
            exObj.ConfigDetails = config.ConfigDetails.Select(cd => new
            {
                cd.Id,
                ConfigId = config.Id,
                PhidgetType = new
                {
                    cd.PhidgetType.Id,
                    cd.PhidgetType.Description
                },
                PhidgetStyleType = new
                {
                    cd.PhidgetStyleType.Id,
                    cd.PhidgetStyleType.Description
                },
                cd.Description,
                cd.Location,
                ResponseType = new
                {
                    cd.ResponseType.Id,
                    cd.ResponseType.Description,
                    cd.ResponseType.IsSystem,
                    ResponseTypeCategory = new
                    {
                        cd.ResponseType.ResponseTypeCategory.Id,
                        cd.ResponseType.ResponseTypeCategory.Description
                    },
                    InteractiveActivityType = (cd.ResponseType.InteractiveActivityTypeId != null) 
                    ? new
                    {
                        cd.ResponseType.InteractiveActivityType.Id,
                        cd.ResponseType.InteractiveActivityType.Description
                    } : null
                }
            }).OrderBy(o => o.PhidgetType.Id);

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs/2/details
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
            exObj.IsActiveEventLog = config.IsActiveEventLog;
            exObj.ConfigDetails = config.ConfigDetails.Select(cd => new
            {
                cd.Id,
                ConfigId = config.Id,
                PhidgetType = new
                {
                    cd.PhidgetType.Id,
                    cd.PhidgetType.Description,
                },
                PhidgetStyleType = new
                {
                    cd.PhidgetStyleType.Id,
                    cd.PhidgetStyleType.Description
                },
                cd.Description,
                cd.Location,
                ResponseType = new
                {
                    cd.ResponseType.Id,
                    cd.ResponseType.Description,
                    cd.ResponseType.IsSystem,
                    ResponseTypeCategory = new
                    {
                        cd.ResponseType.ResponseTypeCategory.Id,
                        cd.ResponseType.ResponseTypeCategory.Description                      
                    },
                    InteractiveActivityType = (cd.ResponseType.InteractiveActivityTypeId != null)
                    ? new
                    {
                        cd.ResponseType.InteractiveActivityType.Id,
                        cd.ResponseType.InteractiveActivityType.Description
                    } : null
                }
            }).OrderBy(o => o.PhidgetType.Id);

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Configs/2/activate
        [Route("{id}/activate")]
        [HttpGet]
        public void Activate(int id)
        {
            _configService.Activate(id);
        }

        // POST: api/Configs
        [HttpPost]
        public int Post([FromBody]Config config)
        {
            return _configService.Post(config);
        }

        // PATCH: api/Configs/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]Config config)
        {
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
