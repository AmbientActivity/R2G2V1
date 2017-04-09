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
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.Get();
            });

            if (mediaList == null) return new DynamicJsonArray(new object[0]);
            if (!mediaList.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = mediaList
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
                                pt.MediaPathType.ShortDescription,
                                pt.MediaPathType.IsSystem,
                                pt.MediaPathType.IsSharable,
                                pt.MediaPathType.IsPreviewable
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize,
                                f.IsLinked
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).OrderBy(o => o.ResponseType.Id).ToArray();

            return new DynamicJsonArray(jArray);
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
                publicMediaFile.MediaPathType.ShortDescription,
                publicMediaFile.MediaPathType.IsSystem,
                publicMediaFile.MediaPathType.IsSharable,
                publicMediaFile.MediaPathType.IsPreviewable
            };
            exObj.MediaFile = new
            {
                publicMediaFile.MediaFile.StreamId,
                publicMediaFile.MediaFile.Filename,
                publicMediaFile.MediaFile.FileType,
                publicMediaFile.MediaFile.FileSize,
                publicMediaFile.IsLinked
            };

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PublicMediaFiles?responseTypeId=1
        [HttpGet]
        public async Task<DynamicJsonArray> GetForResponseType(int responseTypeId)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetForResponseType(responseTypeId);
            });

            if (mediaList == null) return new DynamicJsonArray(new object[0]);
            if (!mediaList.Any()) return new DynamicJsonArray(new object[0]);

            //dynamic exObj = new ExpandoObject();

            //exObj.ResponseType = new
            //{
            //    mediaList.First().ResponseType.Id,
            //    mediaList.First().ResponseType.Description,
            //    ResponseTypeCatgory = new
            //    {
            //        mediaList.First().ResponseType.ResponseTypeCategory.Id,
            //        mediaList.First().ResponseType.ResponseTypeCategory.Description
            //    }
            //};

            var jArray = mediaList
                .GroupBy(pt => pt.MediaPathType)
                .Select(files => new { files.First().MediaPathType, Files = files })
                .Select(pt => new
                {
                    MediaPathType = new
                    {
                        pt.MediaPathType.Id,
                        pt.MediaPathType.Path,
                        pt.MediaPathType.Description,
                        pt.MediaPathType.ShortDescription,
                        pt.MediaPathType.IsSystem,
                        pt.MediaPathType.IsSharable,
                        pt.MediaPathType.IsPreviewable
                    },
                    Files = pt.Files.Select(f => new
                    {
                        f.Id,
                        f.StreamId,
                        f.MediaFile.Filename,
                        f.MediaFile.FileType,
                        f.MediaFile.FileSize,
                        f.IsLinked
                    })
                }).OrderBy(o => o.MediaPathType.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/PublicMediaFiles?mediaPathTypeId=5
        [HttpGet]
        public async Task<DynamicJsonArray> GetForMediaPathType(int mediaPathTypeId)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetForMediaPathType(mediaPathTypeId);
            });

            if (mediaList == null) return new DynamicJsonArray(new object[0]);
            if (!mediaList.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = mediaList
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
                                pt.MediaPathType.ShortDescription,
                                pt.MediaPathType.IsSystem,
                                pt.MediaPathType.IsSharable,
                                pt.MediaPathType.IsPreviewable
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize,
                                f.IsLinked
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/PublicMediaFiles?streamId=0d7434bc-cc81-e611-8aa6-90e6bac7161a=5
        [HttpGet]
        public async Task<DynamicJsonArray> GetForStreamId(Guid streamId)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetForStreamId(streamId);
            });

            if (mediaList == null) return new DynamicJsonArray(new object[0]);
            if (!mediaList.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = mediaList
                .Select(x =>
                    new
                    {
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
                            x.MediaPathType.ShortDescription,
                            x.IsLinked
                        }
                    }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/PublicMediaFiles/files?streamId=0d7434bc-cc81-e611-8aa6-90e6bac7161a=5
        [HttpGet]
        [Route("ids")]
        public async Task<int[]> GetIdsForStreamId(Guid streamId)
        {
            IEnumerable<PublicMediaFile> media = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                media = _publicMediaFileService.GetIdsForStreamId(streamId);
            });

            if (media == null) return new int[0];
            if (!media.Any()) return new int[0];

            return !media.Any()
                ? new int[0]
                : media.Select(x => x.Id).ToArray();
        }

        // GET: api/PublicMediaFiles?isSystem=true
        [HttpGet]
        public async Task<DynamicJsonArray> Get(bool isSystem)
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.Get(isSystem);
            });

            if (mediaList == null) return new DynamicJsonArray(new object[0]);
            if (!mediaList.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = mediaList
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
                                pt.MediaPathType.ShortDescription,
                                pt.MediaPathType.IsSystem,
                                pt.MediaPathType.IsSharable,
                                pt.MediaPathType.IsPreviewable
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize,
                                f.IsLinked
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).OrderBy(o => o.ResponseType.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/PublicMediaFiles/linked
        [HttpGet]
        [Route("linked")]
        public async Task<DynamicJsonArray> GetLinkedPublicMedia()
        {
            IEnumerable<PublicMediaFile> mediaList = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                mediaList = _publicMediaFileService.GetLinked();
            });

            if (mediaList == null) return new DynamicJsonArray(new object[0]);
            if (!mediaList.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = mediaList
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
                                pt.MediaPathType.ShortDescription,
                                pt.MediaPathType.IsSystem,
                                pt.MediaPathType.IsSharable,
                                pt.MediaPathType.IsPreviewable
                            },
                            Files = pt.Files.Select(f => new
                            {
                                f.Id,
                                f.StreamId,
                                f.MediaFile.Filename,
                                f.MediaFile.FileType,
                                f.MediaFile.FileSize,
                                f.IsLinked
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).OrderBy(o => o.ResponseType.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/PublicMediaFiles/linked?streamId=46fda92e-6cee-e611-9cb8-98eecb38d473
        [HttpGet]
        [Route("linked")]
        public async Task<DynamicJsonArray> GetLinkedPublicMedia(Guid streamId)
        {
            IEnumerable<PublicMediaFile> media = new Collection<PublicMediaFile>();

            await Task.Run(() =>
            {
                media = _publicMediaFileService.GetLinked(streamId);
            });

            if (media == null) return new DynamicJsonArray(new object[0]);
            if (media.All(x => x.StreamId != streamId)) return new DynamicJsonArray(new object[0]);

            var jArray = media
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
                            x.MediaPathType.ShortDescription,
                            x.IsLinked
                        }
                    }).ToArray();

            return new DynamicJsonArray(jArray);
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
        public int Post([FromBody]PublicMediaFile publicMediaFile)
        {
            return _publicMediaFileService.Post(publicMediaFile);
        }

        // PATCH: api/PublicMediaFiles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]PublicMediaFile publicMediaFile)
        {
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
