﻿using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
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
    [RoutePrefix("api/responsedetails")]
    public class ResponsesController : ApiController
    {
        private readonly IResponseService _responseService;

        public ResponsesController(IResponseService responseService)
        {
            _responseService = responseService;
        }

        // GET: api/Responses
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<Response> responses = new Collection<Response>();

            await Task.Run(() =>
            {
                responses = _responseService.Get();
            });

            if (responses == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Responses = responses.Select(r => new
            {
                r.Id,
                r.StreamId,
                r.MediaFile.Filename
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Responses/5
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var response = new Response();

            await Task.Run(() =>
            {
                response = _responseService.Get(id);
            });

            if (response == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = response.Id;
            exObj.StreamId = response.StreamId;
            exObj.Filename = response.MediaFile.Filename;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Responses/5   <== ProfileDetailId
        [Route("{id}")]
        [HttpPost]
        public void Post(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<Response>(value);
            
            _responseService.Post(response);
        }

        // PATCH: api/Responses/5
        [Route("{id}")]
        [HttpPatch]
        public void Patch(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Responses/5
        public void Delete(int id)
        {
        }
    }
}
