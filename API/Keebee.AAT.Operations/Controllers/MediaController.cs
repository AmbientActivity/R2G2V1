using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Helpers;
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

        // GET: api/Media/55a32e73-b176-e611-8a92-90e6bac7161a
        public async Task<DynamicJsonObject> Get(Guid id)
        {
            var media = new MediaFile();

            await Task.Run(() =>
            {
                media = _mediaService.Get(id);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.StreamId = media.StreamId;
            exObj.Filename = media.Filename;
            exObj.IsFolder = media.IsFolder;
            exObj.FileSize = media.FileSize;
            exObj.FileType = media.FileType;
            exObj.Path = media.Path;

            return new DynamicJsonObject(exObj);
        }
    }
}
