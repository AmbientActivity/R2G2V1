using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/personalpictures")]
    public class PersonalPicturesController : ApiController
    {
        private readonly IPersonalPictureService _ambientResponseService;

        public PersonalPicturesController(IPersonalPictureService ambientResponseService)
        {
            _ambientResponseService = ambientResponseService;
        }

        // GET: api/PersonalPictures
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<PersonalPicture> pictures = new Collection<PersonalPicture>();

            await Task.Run(() =>
            {
                pictures = _ambientResponseService.Get();
            });

            if (pictures == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.PersonalPictures = pictures.Select(r => new
            {
                r.Id,
                Resident = new
                {
                    r.Resident.Id,
                    r.Resident.FirstName,
                    r.Resident.LastName,
                    r.Resident.Gender,
                    r.Resident.Tag,
                    Profile = new
                    {
                        r.Resident.Profile.Id,
                        r.Resident.Profile.Description,
                        r.Resident.Profile.GameDifficultyLevel
                    }
                },
                r.StreamId,
                FilePath = Path.Combine(r.MediaFile.Path, r.MediaFile.Filename),
                r.MediaFile.FileType,
                r.MediaFile.FileSize
            });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PersonalPictures/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var picture = new PersonalPicture();

            await Task.Run(() =>
            {
                picture = _ambientResponseService.Get(id);
            });

            if (picture == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = picture.Id;
            exObj.Resident = new
                             {
                                 picture.Resident.Id,
                                 picture.Resident.FirstName,
                                 picture.Resident.LastName,
                                 picture.Resident.Gender,
                                 picture.Resident.Tag,
                                 Profile = new
                                 {
                                    picture.Resident.Profile.Id,
                                    picture.Resident.Profile.Description,
                                    picture.Resident.Profile.GameDifficultyLevel
                                 }
                             };
            exObj.StreamId = picture.StreamId;
            exObj.FilePath = Path.Combine(picture.MediaFile.Path, picture.MediaFile.Filename);
            exObj.FileType = picture.MediaFile.FileType;
            exObj.FileSize = picture.MediaFile.FileSize;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/PersonalPictures
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<PersonalPicture>(value);
            _ambientResponseService.Post(response);
        }

        // PUT: api/PersonalPictures/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var picture = serializer.Deserialize<PersonalPicture>(value);
            _ambientResponseService.Patch(id, picture);
        }

        // DELETE: api/PersonalPictures/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _ambientResponseService.Delete(id);
        }
    }
}
