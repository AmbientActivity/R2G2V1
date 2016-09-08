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
    [RoutePrefix("api/configurationDetails")]
    public class ConfigurationDetailsController : ApiController
    {
        private readonly IConfigurationDetailService _configurationDetailService;

        public ConfigurationDetailsController(IConfigurationDetailService configurationDetailService)
        {
            _configurationDetailService = configurationDetailService;
        }

        // GET: api/ConfigurationDetails
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ConfigurationDetail> configurationDetails = new Collection<ConfigurationDetail>();

            await Task.Run(() =>
            {
                configurationDetails = _configurationDetailService.Get();
            });

            if (configurationDetails == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Configurations = configurationDetails.Select(cd => new
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
                    cd.ResponseType.IsInteractive
                }
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ConfigurationDetails/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var configurationDetail = new ConfigurationDetail();

            await Task.Run(() =>
            {
                configurationDetail = _configurationDetailService.Get(id);
            });

            if (configurationDetail == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = configurationDetail.Id;
            exObj.ActivityType = new
            {
                configurationDetail.ActivityType.Id,
                configurationDetail.ActivityType.Description
            };
            exObj.ResponseType = new
            {
                configurationDetail.ResponseType.Id,
                configurationDetail.ResponseType.Description,
                configurationDetail.ResponseType.IsInteractive
            };

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ConfigurationDetails
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var configurationDetail = serializer.Deserialize<ConfigurationDetail>(value);

            return _configurationDetailService.Post(configurationDetail);
        }

        // PUT: api/ConfigurationDetails/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var configurationDetail = serializer.Deserialize<ConfigurationDetail>(value);
            _configurationDetailService.Patch(id, configurationDetail);
        }

        // DELETE: api/ConfigurationDetails/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _configurationDetailService.Delete(id);
        }
    }
}
