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

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/configDetails")]
    public class ConfigDetailsController : ApiController
    {
        private readonly IConfigDetailService _configDetailService;

        public ConfigDetailsController(IConfigDetailService configDetailService)
        {
            _configDetailService = configDetailService;
        }

        // GET: api/ConfigDetails
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ConfigDetail> configDetails = new Collection<ConfigDetail>();

            await Task.Run(() =>
            {
                configDetails = _configDetailService.Get();
            });

            if (configDetails == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ConfigDetails = configDetails.Select(cd => new
                {
                    cd.Id,
                    cd.ConfigId,
                    PhidgetType = new
                    {
                        cd.PhidgetType.Id,
                        cd.PhidgetType.Description
                    },
                    PhidgetStyleType = new
                    {
                        cd.PhidgetStyleType.Id,
                        cd.PhidgetStyleType.Description,
                    },
                    cd.Description,
                    cd.Location,
                    ResponseType = new
                    {
                        cd.ResponseType.Id,
                        cd.ResponseType.Description,
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
                        } : null,
                    }
            }).OrderBy(o => o.PhidgetType.Id);

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ConfigDetails/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var configDetail = new ConfigDetail();

            await Task.Run(() =>
            {
                configDetail = _configDetailService.Get(id);
            });

            if (configDetail == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = configDetail.Id;
            exObj.ConfigId = configDetail.ConfigId;
            exObj.PhidgetType = new
            {
                configDetail.PhidgetType.Id,
                configDetail.PhidgetType.Description
            };
            exObj.PhidgetStyleType = new
            {
                configDetail.PhidgetStyleType.Id,
                configDetail.PhidgetStyleType.Description
            };
            exObj.Description = configDetail.Description;
            exObj.Location = configDetail.Location;
            exObj.ResponseType = new
                {
                    configDetail.ResponseType.Id,
                    configDetail.ResponseType.Description,
                    ResponseTypeCategory = new
                    {
                        configDetail.ResponseType.ResponseTypeCategory.Id,
                        configDetail.ResponseType.ResponseTypeCategory.Description
                    },
                    InteractiveActivityType = (configDetail.ResponseType.InteractiveActivityTypeId != null)
                    ? new
                    {
                        configDetail.ResponseType.InteractiveActivityType.Id,
                        configDetail.ResponseType.InteractiveActivityType.Description
                    } : null
            };

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ConfigDetails
        [HttpPost]
        public int Post([FromBody]ConfigDetail configDetail)
        {
            return _configDetailService.Post(configDetail);
        }

        // PUT: api/ConfigDetails/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]ConfigDetail configDetail)
        {
            _configDetailService.Patch(id, configDetail);
        }

        // DELETE: api/ConfigDetails/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _configDetailService.Delete(id);
        }
    }
}
