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
    public class MediaFilesController : ApiController
    {
       private readonly IMediaFileService _mediaFileService;

       public MediaFilesController(IMediaFileService mediaService)
        {
            _mediaFileService = mediaService;
        }

        // GET: api/MediaFiles
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaFileService.Get();
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());
            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Media = media.GroupBy(m => m.Path)
                .Select(files => new { files.First().Path, Files = files })
                .Select(x => new
                {
                    x.Path,
                    Files = x.Files.Select(f => new
                    {
                                f.StreamId,
                                f.IsFolder,
                                f.Filename,
                                f.FileType,
                                f.FileSize
                     })
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaFiles/55a32e73-b176-e611-8a92-90e6bac7161a
        [HttpGet]
        public async Task<DynamicJsonObject> Get(Guid id)
        {
            var media = new MediaFile();

            await Task.Run(() =>
            {
                media = _mediaFileService.Get(id);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.StreamId = media.StreamId;
            exObj.IsFolder = media.IsFolder;
            exObj.Filename = media.Filename;
            exObj.FileSize = media.FileSize;
            exObj.FileType = media.FileType;
            exObj.Path = media.Path;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaFiles?residentId=5
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResident(int residentId)
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaFileService.GetForResident(residentId);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());
            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Media = media.GroupBy(m => m.Path)
                .Select(files => new { files.First().Path, Files = files })
                .Select(x => new
                {
                    x.Path,
                    Files = x.Files.Select(f => new
                    {
                        f.StreamId,
                        f.IsFolder,
                        f.Filename,
                        f.FileType,
                        f.FileSize
                    })
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaFiles?path=Exports\EventLog
        [HttpGet]
        public async Task<DynamicJsonObject> GetForPath(string path)
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaFileService.GetForPath(path);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());
            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Media = media.GroupBy(m => m.Path)
                .Select(files => new { files.First().Path, Files = files })
                .Select(x => new
                {
                    x.Path,
                    Files = x.Files.Select(f => new
                    {
                        f.StreamId,
                        f.IsFolder,
                        f.Filename,
                        f.FileType,
                        f.FileSize
                    })
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaFiles?residentId=5&path=music
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResidentPath(int residentId, string path)
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaFileService.GetForResidentPath(residentId, path);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());
            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Path = media.First().Path;
            exObj.Files = media.Select(m => new
            {
                m.StreamId,
                m.IsFolder,
                m.Filename,
                m.FileType,
                m.FileSize
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/MediaFiles?&path=0\images&filename=2008_01_21.jpg
        [HttpGet]
        public async Task<DynamicJsonObject> GetSingleFromPath(string path, string filename)
        {
            var media = new MediaFile();

            await Task.Run(() =>
            {
                media = _mediaFileService.GetSingleFromPath(path, filename);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.StreamId = media.StreamId;
            exObj.Filename = media.Filename;
            exObj.FileType = media.FileType;
            exObj.FileSize = media.FileSize;

            return new DynamicJsonObject(exObj);
        }
    }
}
