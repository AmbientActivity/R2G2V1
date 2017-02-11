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
    [RoutePrefix("api/SystemMediaFiles")]
    public class SystemMediaFilesController : ApiController
    {
        private readonly ISystemMediaFileService _systemMediaFileService;

        public SystemMediaFilesController(ISystemMediaFileService systemMediaFileService)
        {
            _systemMediaFileService = systemMediaFileService;
        }

        // GET: api/SystemMediaFiles
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<SystemMediaFile> mediaList = new Collection<SystemMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _systemMediaFileService.Get();
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

        // GET: api/SystemMediaFiles/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var systemMediaFile = new SystemMediaFile();

            await Task.Run(() =>
            {
                systemMediaFile = _systemMediaFileService.Get(id);
            });

            if (systemMediaFile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = systemMediaFile.Id;
            exObj.ResponseType = new
            {
                systemMediaFile.ResponseType.Id,
                systemMediaFile.ResponseType.Description
            };
            exObj.MediaPathType = new
            {
                systemMediaFile.MediaPathType.Id,
                systemMediaFile.MediaPathType.Path,
                systemMediaFile.MediaPathType.Description,
                systemMediaFile.MediaPathType.ShortDescription
            };
            exObj.MediaFile = new
            {
                systemMediaFile.MediaFile.StreamId,
                systemMediaFile.MediaFile.Filename,
                systemMediaFile.MediaFile.FileType,
                systemMediaFile.MediaFile.FileSize
            };

            return new DynamicJsonObject(exObj);
        }

        // GET: api/SystemMediaFiles?responseTypeId=1
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResponseType(int responseTypeId)
        {
            IEnumerable<SystemMediaFile> mediaList = new Collection<SystemMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _systemMediaFileService.GetForResponseType(responseTypeId);
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

        // GET: api/SystemMediaFiles?mediaPathTypeId=5
        [HttpGet]
        public async Task<DynamicJsonObject> GetForMediaPathType(int mediaPathTypeId)
        {
            IEnumerable<SystemMediaFile> mediaList = new Collection<SystemMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _systemMediaFileService.GetForMediaPathType(mediaPathTypeId);
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

        // GET: api/SystemMediaFiles?responseTypeId=1&filename=photo.jpg
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResponseTypeFilename(int responseTypeId, string filename)
        {
            SystemMediaFile mediaFile = null;

            await Task.Run(() =>
            {
                mediaFile = _systemMediaFileService.GetForResponseTypeFilename(responseTypeId, filename);
            });

            if (mediaFile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = mediaFile.Id;
            exObj.StreamId = mediaFile.StreamId;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/SystemMediaFiles
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var systemMediaFile = serializer.Deserialize<SystemMediaFile>(value);
            _systemMediaFileService.Post(systemMediaFile);
        }

        // PATCH: api/SystemMediaFiles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var systemMediaFile = serializer.Deserialize<SystemMediaFile>(value);
            _systemMediaFileService.Patch(id, systemMediaFile);
        }

        // DELETE: api/SystemMediaFiles/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _systemMediaFileService.Delete(id);
        }
    }
}
