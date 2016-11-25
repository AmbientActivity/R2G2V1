using System;
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
    [RoutePrefix("api/PublicMediaFiles")]
    public class PublicMediaFilesController : ApiController
    {
        private readonly IPublicMediaFileService _publicMediaFileService;

        public PublicMediaFilesController(IPublicMediaFileService publicMediaFileService)
        {
            _publicMediaFileService = publicMediaFileService;
        }

        // GET: api/PublicMediaFiles
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.Get();
            });

            if (mediaList == null) return new DynamicJsonObject(new ExpandoObject());
            if (!mediaList.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.MediaFiles = mediaList
                .GroupBy(rt => rt.ResponseType)
                .Select(mediaFiles => new { mediaFiles.First().ResponseType, MediaFiles = mediaFiles })
                .Select(mf => new
                {
                    ResponseType = new
                    {
                        mf.ResponseType.Id,
                        mf.ResponseType.Description,
                        ResponseTypeCatgory = new
                        {
                            mf.ResponseType.ResponseTypeCategory.Id,
                            mf.ResponseType.ResponseTypeCategory.Description
                        }
                    },
                    Paths = mf.MediaFiles
                        .GroupBy(pt => pt.MediaPathType)
                        .Select(files => new { files.First().MediaPathType, Files = files })
                        .Select(pt => new
                        {
                            MediaPathType = new
                            {
                                pt.MediaPathType.Id,
                                pt.MediaPathType.Path,
                                pt.MediaPathType.Description,
                                pt.MediaPathType.ShortDescription
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).OrderBy(o => o.ResponseType.Id);

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PublicMediaFiles/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var publicMediaFile = new PublicMediaFile();

            await Task.Run(() =>
            {
                publicMediaFile = _publicMediaFileService.Get(id);
            });

            if (publicMediaFile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = publicMediaFile.Id;
            exObj.ResponseType = new
                {
                    publicMediaFile.ResponseType.Id,
                    publicMediaFile.ResponseType.Description
                };
            exObj.MediaPathType = new
            {
                publicMediaFile.MediaPathType.Id,
                publicMediaFile.MediaPathType.Path,
                publicMediaFile.MediaPathType.Description,
                publicMediaFile.MediaPathType.ShortDescription
            };
            exObj.MediaFile = new
                {
                    publicMediaFile.MediaFile.StreamId,
                    publicMediaFile.MediaFile.Filename,
                    publicMediaFile.MediaFile.FileType,
                    publicMediaFile.MediaFile.FileSize,
                };

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PublicMediaFiles?responseTypeId=1
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResponseType(int responseTypeId)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetForResponseType(responseTypeId);
            });

            if (mediaList == null) return new DynamicJsonObject(new ExpandoObject());
            if (!mediaList.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.MediaResponseType = mediaList
                .GroupBy(rt => rt.ResponseType)
                .Select(mediaFiles => new { mediaFiles.First().ResponseType, MediaFiles = mediaFiles })
                .Select(mf => new
                {
                    ResponseType = new
                    {
                        mf.ResponseType.Id,
                        mf.ResponseType.Description,
                        ResponseTypeCatgory = new
                        {
                            mf.ResponseType.ResponseTypeCategory.Id,
                            mf.ResponseType.ResponseTypeCategory.Description
                        }
                    },
                    Paths = mf.MediaFiles
                        .GroupBy(pt => pt.MediaPathType)
                        .Select(files => new { files.First().MediaPathType, Files = files })
                        .Select(pt => new
                        {
                            MediaPathType = new
                            {
                                pt.MediaPathType.Id,
                                pt.MediaPathType.Path,
                                pt.MediaPathType.Description,
                                pt.MediaPathType.ShortDescription
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).SingleOrDefault();

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PublicMediaFiles?mediaPathTypeId=5
        [HttpGet]
        public async Task<DynamicJsonObject> GetForMediaPathType(int mediaPathTypeId)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetForMediaPathType(mediaPathTypeId);
            });

            if (mediaList == null) return new DynamicJsonObject(new ExpandoObject());
            if (!mediaList.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.MediaFiles = mediaList
                .GroupBy(rt => rt.ResponseType)
                .Select(mediaFiles => new { mediaFiles.First().ResponseType, MediaFiles = mediaFiles })
                .Select(mf => new
                {
                    ResponseType = new
                    {
                        mf.ResponseType.Id,
                        mf.ResponseType.Description
                    },
                    Paths = mf.MediaFiles
                        .GroupBy(pt => pt.MediaPathType)
                        .Select(files => new { files.First().MediaPathType, Files = files })
                        .Select(pt => new
                        {
                            MediaPathType = new
                            {
                                pt.MediaPathType.Id,
                                pt.MediaPathType.Path,
                                pt.MediaPathType.Description,
                                pt.MediaPathType.ShortDescription
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PublicMediaFiles?streamId=0d7434bc-cc81-e611-8aa6-90e6bac7161a=5
        [HttpGet]
        public async Task<DynamicJsonObject> GetForStreamId(Guid streamId)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetForStreamId(streamId);
            });

            if (mediaList == null) return new DynamicJsonObject(new ExpandoObject());
            if (!mediaList.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.MediaFiles = mediaList
                .Select(x =>
                    new
                    {
                        x.Id,
                        x.StreamId,
                        ResponseType = new
                            {
                                x.ResponseType.Id,
                                x.ResponseType.Description
                            },
                        MediaPathType = new
                            {
                                x.MediaPathType.Id,
                                x.MediaPathType.Path,
                                x.MediaPathType.Description,
                                x.MediaPathType.ShortDescription
                        }
                    });

            return new DynamicJsonObject(exObj);
        }


        // GET: api/PublicMediaFiles?responseTypeId=1&filename=photo.jpg
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResponseTypeFilename(int responseTypeId, string filename)
        {
            PublicMediaFile mediaFile = null;

            await Task.Run(() =>
            {
                mediaFile = _publicMediaFileService.GetForResponseTypeFilename(responseTypeId, filename);
            });

            if (mediaFile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = mediaFile.Id;
            exObj.StreamId = mediaFile.StreamId;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/PublicMediaFiles
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var publicMediaFile = serializer.Deserialize<PublicMediaFile>(value);
            _publicMediaFileService.Post(publicMediaFile);
        }

        // PATCH: api/PublicMediaFiles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var publicMediaFile = serializer.Deserialize<PublicMediaFile>(value);
            _publicMediaFileService.Patch(id, publicMediaFile);
        }

        // DELETE: api/PublicMediaFiles/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _publicMediaFileService.Delete(id);
        }
    }
}
