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
    [RoutePrefix("api/ResidentMediaFiles")]
    public class ResidentMediaFilesController : ApiController
    {
        private readonly IResidentMediaFileService _residentMediaFileService;

        public ResidentMediaFilesController(IResidentMediaFileService residentMediaFileService)
        {
            _residentMediaFileService = residentMediaFileService;
        }

        // GET: api/ResidentMediaFiles
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.Get();
            });

            if (media == null) return new DynamicJsonArray(new object[0]);
            if (!media.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = media.GroupBy(m => m.Resident)
                .Select(files => new { files.First().Resident, Files = files })
                .Select(x => new
                {
                    Resident = new
                    {
                        x.Resident.Id,
                        x.Resident.FirstName,
                        x.Resident.LastName,
                        x.Resident.Gender,
                        x.Resident.GameDifficultyLevel
                    },
                    MediaResponseTypes = x.Files
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
                                                f.MediaFile.FileSize,
                                                f.IsLinked
                                    })
                                    }).OrderBy(o => o.MediaPathType.Id)
                        }).OrderBy(o => o.ResponseType.Id)
                }).OrderBy(o => o.Resident.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/ResidentMediaFiles/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var residentMediaFile = new ResidentMediaFile();

            await Task.Run(() =>
            {
                residentMediaFile = _residentMediaFileService.Get(id);
            });

            if (residentMediaFile == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = residentMediaFile.Id;
            exObj.Resident = new
                {
                    residentMediaFile.Resident.Id,
                    residentMediaFile.Resident.FirstName,
                    residentMediaFile.Resident.LastName,
                    residentMediaFile.Resident.Gender,
                    residentMediaFile.Resident.GameDifficultyLevel
                };
            exObj.ResponseType = new
                {
                    residentMediaFile.ResponseType.Id,
                    residentMediaFile.ResponseType.Description,
                    ResponseTypeCatgory = new
                        {
                            residentMediaFile.ResponseType.ResponseTypeCategory.Id,
                            residentMediaFile.ResponseType.ResponseTypeCategory.Description
                        }
                };
            exObj.MediaPathType = new
                {
                    residentMediaFile.MediaPathType.Id,
                    residentMediaFile.MediaPathType.Path,
                    residentMediaFile.MediaPathType.Description,
                    residentMediaFile.MediaPathType.ShortDescription
                };
            exObj.MediaFile = new
                {
                    residentMediaFile.MediaFile.StreamId,
                    residentMediaFile.MediaFile.Filename,
                    residentMediaFile.MediaFile.FileType,
                    residentMediaFile.MediaFile.FileSize,
                    residentMediaFile.IsLinked
            };

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ResidentMediaFiles?residentId=5
        [HttpGet]
        public async Task<DynamicJsonArray> GetForResident(int residentId)
        {
            IEnumerable<ResidentMediaFile> residentMedia = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                residentMedia = _residentMediaFileService.GetForResident(residentId);
            });

            if (!residentMedia.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = residentMedia
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
                                f.MediaFile.FileSize,
                                f.IsLinked
                            })
                        }).OrderBy(o => o.MediaPathType.Id)
                }).OrderBy(o => o.ResponseType.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/ResidentMediaFiles?residentId=2&responseTypeId=6
        [HttpGet]
        public async Task<DynamicJsonArray> GetForResidentResponseType(int residentId, int responseTypeId)
        {
            IEnumerable<ResidentMediaFile> residentMedia = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                residentMedia = _residentMediaFileService.GetForResidentResponseType(residentId, responseTypeId);
            });

            if (residentMedia == null) return new DynamicJsonArray(new object[0]);
            if (!residentMedia.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = residentMedia
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
                        f.MediaFile.FileSize,
                        f.IsLinked
                    })
                }).OrderBy(o => o.MediaPathType.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/ResidentMediaFiles/ids?streamId=abaad215-95ed-e611-9cb8-98eecb38d473
        [HttpGet]
        [Route("ids")]
        public async Task<int[]> GetIdsForStreamId(Guid streamId)
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.GetIdsForStreamId(streamId);
            });

            if (media == null) return new int[0];

            return !media.Any() 
                ? new int[0] 
                : media.Select(x => x.Id).ToArray();
        }

        // GET: api/ResidentMediaFiles/linked
        [HttpGet]
        [Route("linked")]
        public async Task<DynamicJsonArray> GetLinkedResidentMedia()
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.GetLinked();
            });

            if (media == null) return new DynamicJsonArray(new object[0]);
            if (!media.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = media.GroupBy(m => m.Resident)
                .Select(files => new {files.First().Resident, Files = files})
                .Select(x => new
                {
                    Resident = new
                    {
                        x.Resident.Id,
                        x.Resident.FirstName,
                        x.Resident.LastName,
                        x.Resident.Gender,
                        x.Resident.GameDifficultyLevel
                    },
                    ResponseTypesPaths = x.Files
                        .GroupBy(rt => rt.ResponseType)
                        .Select(mediaFiles => new {mediaFiles.First().ResponseType, MediaFiles = mediaFiles})
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
                                .Select(files => new {files.First().MediaPathType, Files = files})
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
                                        f.MediaFile.FileSize,
                                        f.IsLinked
                                    })
                                }).OrderBy(o => o.MediaPathType.Id)
                        }).OrderBy(o => o.ResponseType.Id)
                    }).OrderBy(o => o.Resident.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/ResidentMediaFiles/linked?streamId=46fda92e-6cee-e611-9cb8-98eecb38d473
        [HttpGet]
        [Route("linked")]
        public async Task<DynamicJsonArray> GetLinkedResidentMedia(Guid streamId)
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.GetLinked(streamId);
            });

            if (media == null) return new DynamicJsonArray(new object[0]);
            if (!media.Any()) return new DynamicJsonArray(new object[0]);

            var jArray = media.GroupBy(m => m.Resident)
                .Select(files => new { files.First().Resident, Files = files })
                .Select(x => new
                {
                    Resident = new
                    {
                        x.Resident.Id,
                        x.Resident.FirstName,
                        x.Resident.LastName,
                        x.Resident.Gender,
                        x.Resident.GameDifficultyLevel
                    },
                    ResponseTypesPaths = x.Files
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
                                        f.MediaFile.FileSize,
                                        f.IsLinked
                                    })
                                }).OrderBy(o => o.MediaPathType.Id)
                        }).OrderBy(o => o.ResponseType.Id)
                }).OrderBy(o => o.Resident.Id).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // POST: api/ResidentMediaFiles
        [HttpPost]
        public int Post([FromBody]ResidentMediaFile residentMediaFile)
        {
            return _residentMediaFileService.Post(residentMediaFile);
        }

        // PATCH: api/ResidentMediaFiles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]ResidentMediaFile residentMediaFile)
        {
            _residentMediaFileService.Patch(id, residentMediaFile);
        }

        // DELETE: api/ResidentMediaFiles/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _residentMediaFileService.Delete(id);
        }
    }
}
