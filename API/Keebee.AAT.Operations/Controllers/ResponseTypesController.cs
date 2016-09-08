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
    [RoutePrefix("api/ResponseTypes")]
    public class ResponseTypesController : ApiController
    {
        private readonly IResponseTypeService _responseTypeService;

        public ResponseTypesController(IResponseTypeService responseTypeService)
        {
            _responseTypeService = responseTypeService;
        }

        // GET: api/ResponseTypes
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ResponseType> responseTypes = new Collection<ResponseType>();

            await Task.Run(() =>
                {
                    responseTypes = _responseTypeService.Get();
                });

            if (responseTypes == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ResponseTypes = responseTypes
                .Select(x => new
                {
                    x.Id,
                    ResponseTypeCategory = x.ResponseTypeCategory.Description,
                    x.Description,
                    ResponseTYpeCategory = new
                        {
                            x.ResponseTypeCategory.Id,
                            x.ResponseTypeCategory.Description                
                        },
                    x.IsInteractive,
                    x.IsSystem
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ResponseTypes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var responseType = new ResponseType();

            await Task.Run(() =>
            {
                responseType = _responseTypeService.Get(id);
            });

            if (responseType == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = responseType.Id;
            exObj.Description = responseType.Description;
            exObj.ResponseTypeCategory = new
                {
                    responseType.ResponseTypeCategory.Id,
                    responseType.ResponseTypeCategory.Description,
                };
            exObj.IsInteractive = responseType.IsInteractive;
            exObj.IsSystem = responseType.IsSystem;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ResponseTypes
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var responseType = serializer.Deserialize<ResponseType>(value);
            _responseTypeService.Post(responseType);
        }

        // PATCH: api/ResponseTypes/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var responseType = serializer.Deserialize<ResponseType>(value);
            _responseTypeService.Patch(id, responseType);
        }

        // DELETE: api/ResponseTypes/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _responseTypeService.Delete(id);
        }
    }
}