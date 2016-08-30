using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    public class MediaController : ApiController
    {
       private readonly IMediaService _mediaService;

       public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

       // GET: api/Media
       public async Task<IEnumerable<string>> Get()
        {
           IEnumerable<MediaFile> media = new Collection<MediaFile>();
           ICollection<string> filePaths = new Collection<string>();

           await Task.Run(() =>
           {
               media = _mediaService.Get();
           });

           foreach (var m in media)
           {
               filePaths.Add(Path.Combine(m.Path, m.Filename));
           }

           return filePaths;
        }

        // GET: api/Media/5
        public async Task<string> Get(Guid id)
        {
            var media = new MediaFile();
            string filePath = string.Empty;

            await Task.Run(() =>
            {
                media = _mediaService.Get(id);
            });

            filePath = Path.Combine(media.Path, media.Filename);

            return filePath;
        }

        // POST: api/Media
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Media/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Media/5
        public void Delete(int id)
        {
        }
    }
}
