using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
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
       public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaService.Get();
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
                                m.Path
                            });

            return new DynamicJsonObject(exObj);
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
            exObj.IsFolder = media.IsFolder;
            exObj.Filename = media.Filename;
            exObj.FileSize = media.FileSize;
            exObj.FileType = media.FileType;
            exObj.Path = media.Path;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Media?profileId=5
        public async Task<DynamicJsonObject> GetForProfile(int profileId)
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaService.GetForProfile(profileId);
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
                m.Path
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Media/Videos?profileId=5&folder=music
        public async Task<DynamicJsonObject> GetForProfileFolder(int profileId, string folder)
        {
            IEnumerable<MediaFile> media = new Collection<MediaFile>();

            await Task.Run(() =>
            {
                media = _mediaService.GetForProfileFolder(profileId, folder);
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
                m.Path
            });

            return new DynamicJsonObject(exObj);
        }
    }
}
