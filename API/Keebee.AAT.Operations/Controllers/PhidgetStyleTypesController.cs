﻿using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
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
    [RoutePrefix("api/PhidgetStyleTypes")]
    public class PhidgetStyleTypesController : ApiController
    {
        private readonly IPhidgetStyleTypeService _phidgetStyleTypeService;

        public PhidgetStyleTypesController(IPhidgetStyleTypeService phidgetStyleTypeService)
        {
            _phidgetStyleTypeService = phidgetStyleTypeService;
        }

        // GET: api/PhidgetStyleTypes
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<PhidgetStyleType> phidgetStyleTypes = new Collection<PhidgetStyleType>();

            await Task.Run(() =>
            {
                phidgetStyleTypes = _phidgetStyleTypeService.Get();
            });

            if (phidgetStyleTypes == null) return new DynamicJsonArray(new object[0]);

            var jArray = phidgetStyleTypes
                .Select(x => new
                {
                    x.Id,
                    x.Description
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/PhidgetStyleTypes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var phidgetStyleType = new PhidgetStyleType();

            await Task.Run(() =>
            {
                phidgetStyleType = _phidgetStyleTypeService.Get(id);
            });

            if (phidgetStyleType == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = phidgetStyleType.Id;
            exObj.Description = phidgetStyleType.Description;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/PhidgetStyleTypes
        [HttpPost]
        public void Post([FromBody]PhidgetStyleType phidgetStyleType)
        {
            _phidgetStyleTypeService.Post(phidgetStyleType);
        }

        // PATCH: api/PhidgetStyleTypes/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]PhidgetStyleType phidgetStyleType)
        {
            _phidgetStyleTypeService.Patch(id, phidgetStyleType);
        }

        // DELETE: api/PhidgetStyleTypes/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _phidgetStyleTypeService.Delete(id);
        }
    }
}
