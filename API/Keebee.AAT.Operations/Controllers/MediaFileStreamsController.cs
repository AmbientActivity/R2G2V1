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
    public class MediaFileStreamsController : ApiController
    {
       private readonly IMediaFileStreamService _mediaFileStreamService;

       public MediaFileStreamsController(IMediaFileStreamService mediaService)
        {
            _mediaFileStreamService = mediaService;
        }

        // GET: api/MediaFileStreams/55a32e73-b176-e611-8a92-90e6bac7161a
        public async Task<DynamicJsonObject> Get(Guid id)
        {
            var media = new MediaFileStream();

            await Task.Run(() =>
            {
                media = _mediaFileStreamService.Get(id);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.StreamId = media.StreamId;
            exObj.IsFolder = media.IsFolder;
            exObj.Filename = media.Filename;
            exObj.FileSize = media.FileSize;
            exObj.FileType = media.FileType;
            exObj.Stream = media.Stream;
            exObj.Path = media.Path;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaFileStreams?path=Exports\EventLog
        public async Task<DynamicJsonObject> GetForPath(string path)
        {
            IEnumerable<MediaFileStream> media = new Collection<MediaFileStream>();

            await Task.Run(() =>
            {
                media = _mediaFileStreamService.GetForPath(path);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Media = media.Select(m => new
            {
                m.StreamId,
                m.IsFolder,
                m.Filename,
                m.FileType,
                m.FileSize,
                m.Path,
                m.Stream
            });

            return new DynamicJsonObject(exObj);
        }
    }
}
