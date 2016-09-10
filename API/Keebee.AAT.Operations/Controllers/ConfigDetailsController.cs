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
            exObj.Configurations = configDetails.Select(cd => new
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
                    cd.ResponseType.IsInteractive
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
            exObj.PhidgetType = new
            {
                configDetail.PhidgetType.Id,
                configDetail.PhidgetType.Description
            };
            exObj.Description = configDetail.Description;
            exObj.ResponseType = new
            {
                configDetail.ResponseType.Id,
                configDetail.ResponseType.Description,
                configDetail.ResponseType.IsInteractive
            };

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ConfigDetails
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var configDetail = serializer.Deserialize<ConfigDetail>(value);

            return _configDetailService.Post(configDetail);
        }

        // PUT: api/ConfigDetails/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var configDetail = serializer.Deserialize<ConfigDetail>(value);
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
