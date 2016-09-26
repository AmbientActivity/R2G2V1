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
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.Get();
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());
            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.ResidentMedia = media.GroupBy(m => m.Resident)
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
                    MediaFiles = x.Files
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
                                            pt.MediaPathType.Description
                                        },
                                    Files = pt.Files.Select(f => new
                                        {
                                                f.Id,
                                                f.StreamId,
                                                f.MediaFile.Filename,
                                                f.MediaFile.FileType,
                                                f.MediaFile.FileSize,
                                                f.IsPublic
                                        })
                                    }).OrderBy(o => o.MediaPathType.Id)
                        }).OrderBy(o => o.ResponseType.Id)
                }).OrderBy(o => o.Resident.Id);

            return new DynamicJsonObject(exObj);
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
            exObj.Path = residentMediaFile.MediaPathType.Description;
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
                    residentMediaFile.MediaPathType.Description
                };
            exObj.MediaFile = new
                {
                    residentMediaFile.MediaFile.StreamId,
                    residentMediaFile.MediaFile.Filename,
                    residentMediaFile.MediaFile.FileType,
                    residentMediaFile.MediaFile.FileSize,
                    residentMediaFile.IsPublic
                };

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ResidentMediaFiles?residentId=5
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResident(int residentId)
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.GetForResident(residentId);
            });

            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.ResidentMedia = media.GroupBy(m => m.Resident)
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
                    MediaFiles = x.Files
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
                                        pt.MediaPathType.Description
                                    },
                                    Files = pt.Files.Select(f => new
                                    {
                                        f.Id,
                                        f.StreamId,
                                        f.MediaFile.Filename,
                                        f.MediaFile.FileType,
                                        f.MediaFile.FileSize,
                                        f.IsPublic
                                    })
                                }).OrderBy(o => o.MediaPathType.Id)
                        }).OrderBy(o => o.ResponseType.Id)
                }).Single();

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ResidentMediaFiles?residentId=2&responseTypeId=6
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResidentResponseType(int residentId, int responseTypeId)
        {
            IEnumerable<ResidentMediaFile> media = new Collection<ResidentMediaFile>();

            await Task.Run(() =>
            {
                media = _residentMediaFileService.GetForResidentResponseType(residentId, responseTypeId);
            });

            if (media == null) return new DynamicJsonObject(new ExpandoObject());
            if (!media.Any()) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.ResidentMedia = media.GroupBy(m => m.Resident)
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
                    MediaFiles = x.Files
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
                                        pt.MediaPathType.Description
                                    },
                                    Files = pt.Files.Select(f => new
                                    {
                                        f.Id,
                                        f.StreamId,
                                        f.MediaFile.Filename,
                                        f.MediaFile.FileType,
                                        f.MediaFile.FileSize,
                                        f.IsPublic
                                    })
                                }).OrderBy(o => o.MediaPathType.Id)
                        }).OrderBy(o => o.ResponseType.Id)
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ResidentMediaFiles
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var residentMediaFile = serializer.Deserialize<ResidentMediaFile>(value);
            return _residentMediaFileService.Post(residentMediaFile);
        }

        // PATCH: api/ResidentMediaFiles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var residentMediaFile = serializer.Deserialize<ResidentMediaFile>(value);
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
