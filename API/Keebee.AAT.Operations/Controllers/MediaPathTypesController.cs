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
    [RoutePrefix("api/MediaPathTypes")]
    public class MediaPathTypesController : ApiController
    {
        private readonly IMediaPathTypeService _mediaPathTypeService;

        public MediaPathTypesController(IMediaPathTypeService mediaPathTypeService)
        {
            _mediaPathTypeService = mediaPathTypeService;
        }

        // GET: api/MediaPathTypes
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<MediaPathType> mediaPathTypes = new Collection<MediaPathType>();

            await Task.Run(() =>
            {
                mediaPathTypes = _mediaPathTypeService.Get();
            });

            if (mediaPathTypes == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.MediaPathTypes = mediaPathTypes
                .Select(x => new
                {
                    x.Id,
                    x.Path,
                    x.Description,
                    x.ShortDescription,
                    x.IsPreviewable,
                    x.IsSystem,
                    x.IsSharable
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaPathTypes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var mediaPathType = new MediaPathType();

            await Task.Run(() =>
            {
                mediaPathType = _mediaPathTypeService.Get(id);
            });

            if (mediaPathType == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = mediaPathType.Id;
            exObj.Path = mediaPathType.Path;
            exObj.Description = mediaPathType.Description;
            exObj.ShortDescription = mediaPathType.ShortDescription;
            exObj.IsSystem = mediaPathType.IsPreviewable;
            exObj.IsSystem = mediaPathType.IsSystem;
            exObj.IsSharable = mediaPathType.IsSharable;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaPathTypes?isSystem=false
        [HttpGet]
        public async Task<DynamicJsonObject> Get(bool isSystem)
        {
            IEnumerable<MediaPathType> mediaPathTypes = new Collection<MediaPathType>();

            await Task.Run(() =>
            {
                mediaPathTypes = _mediaPathTypeService.Get(isSystem);
            });

            if (mediaPathTypes == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.MediaPathTypes = mediaPathTypes
                .Select(x => new
                {
                    x.Id,
                    x.Path,
                    x.Description,
                    x.ShortDescription,
                    x.IsPreviewable,
                    x.IsSystem,
                    x.IsSharable
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/MediaPathTypes
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var mediaPathType = serializer.Deserialize<MediaPathType>(value);
            _mediaPathTypeService.Post(mediaPathType);
        }

        // PATCH: api/MediaPathTypes/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var mediaPathType = serializer.Deserialize<MediaPathType>(value);
            _mediaPathTypeService.Patch(id, mediaPathType);
        }

        // DELETE: api/MediaPathTypes/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _mediaPathTypeService.Delete(id);
        }
    }
}
