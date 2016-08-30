using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IPersonalPictureService
    {
        IEnumerable<PersonalPicture> Get();
        PersonalPicture Get(int id);
        void Post(PersonalPicture response);
        void Patch(int id, PersonalPicture response);
        void Delete(int id);
    }

    public class PersonalPictureService : IPersonalPictureService
    {
        public IEnumerable<PersonalPicture> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var responses = container.PersonalPictures
                .Expand("Resident($expand=Profile),MediaFile")
                .AsEnumerable();

            return responses;
        }

        public PersonalPicture Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var picture = container.PersonalPictures.ByKey(id)
                .Expand("Resident($expand=Profile),MediaFile")
                .GetValue();

            return picture;
        }

        public void Post(PersonalPicture picture)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToPersonalPictures(picture);
            container.SaveChanges();
        }

        public void Patch(int id, PersonalPicture picture)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var p = container.PersonalPictures.Where(e => e.Id == id).SingleOrDefault();
            if (p == null) return;

            if (picture.ResidentId != null)
                p.ResidentId = picture.ResidentId;

            container.UpdateObject(p);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var response = container.PersonalPictures.Where(e => e.Id == id).SingleOrDefault();
            if (response == null) return;

            container.DeleteObject(response);
            container.SaveChanges();
        }
    }
}