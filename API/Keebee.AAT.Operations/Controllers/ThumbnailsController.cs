using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/thumbnails")]
    public class ThumbnailsController : ApiController
    {
        private readonly IThumbnailService _thumbnailService;

        public ThumbnailsController(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        // GET: api/Thumbnails
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<Thumbnail> thumbnails = new Collection<Thumbnail>();

            await Task.Run(() =>
            {
                thumbnails = _thumbnailService.Get();
            });

            if (thumbnails == null) return new DynamicJsonArray(new object[0]);

            var jArray = thumbnails
                .Select(t => new
                {
                    t.StreamId,
                    t.Image
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/Thumbnails/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(Guid id)
        {
            var thumbnail = new Thumbnail();

            await Task.Run(() =>
            {
                thumbnail = _thumbnailService.Get(id);
            });

            if (thumbnail == null) return null;

            dynamic exObj = new ExpandoObject();
            exObj.StreamId = thumbnail.StreamId;
            exObj.Image = thumbnail.Image;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Thumbnails?streamId=C69352E4-704C-E711-9CCF-98EECB38D473
        [HttpGet]
        [Route("image")]
        public async Task<byte[]> GetImage(Guid streamId)
        {
            byte[] byteArray = null;

            await Task.Run(() =>
            {
                byteArray = _thumbnailService.GetImage(streamId);
            });

            return byteArray;
        }

        // POST: api/Thumbnails
        [HttpPost]
        public void Post([FromBody]Thumbnail thumbnail)
        {
            _thumbnailService.Post(thumbnail);
        }

        // PUT: api/Thumbnails/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(Guid id, [FromBody]Thumbnail thumbnail)
        {
            _thumbnailService.Patch(id, thumbnail);
        }

        // DELETE: api/Thumbnails/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(Guid id)
        {
            _thumbnailService.Delete(id);
        }
    }
}
