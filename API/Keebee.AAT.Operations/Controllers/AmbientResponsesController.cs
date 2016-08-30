using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/ambientresponses")]
    public class AmbientResponsesController : ApiController
    {
        private readonly IAmbientResponseService _ambientResponseService;

        public AmbientResponsesController(IAmbientResponseService ambientResponseService)
        {
            _ambientResponseService = ambientResponseService;
        }

        // GET: api/AmbientResponses
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<AmbientResponse> responses = new Collection<AmbientResponse>();

            await Task.Run(() =>
            {
                responses = _ambientResponseService.Get();
            });

            if (responses == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.AmbientResponses = responses.Select(r => new
            {
                r.Id,
                r.StreamId,
                r.ResponseTypeId,
                FilePath = Path.Combine(r.MediaFile.Path, r.MediaFile.Filename),
                r.MediaFile.FileType,
                r.MediaFile.FileSize
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/AmbientResponses/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var response = new AmbientResponse();

            await Task.Run(() =>
            {
                response = _ambientResponseService.Get(id);
            });

            if (response == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = response.Id;
            exObj.StreamId = response.StreamId;
            exObj.ResponseTypeId = response.ResponseTypeId;
            exObj.FilePath = Path.Combine(response.MediaFile.Path, response.MediaFile.Filename);
            exObj.FileType = response.MediaFile.FileType;
            exObj.FileSize = response.MediaFile.FileSize;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/AmbientResponses
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<AmbientResponse>(value);
            _ambientResponseService.Post(response);
        }

        // PUT: api/AmbientResponses/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<AmbientResponse>(value);
            _ambientResponseService.Patch(id, response);
        }

        // DELETE: api/AmbientResponses/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _ambientResponseService.Delete(id);
        }
    }
}
