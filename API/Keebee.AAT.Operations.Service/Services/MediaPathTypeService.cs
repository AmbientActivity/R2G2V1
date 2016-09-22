using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IMediaPathTypeService
    {
        IEnumerable<MediaPathType> Get();
        MediaPathType Get(int id);
        void Post(MediaPathType mediaPathType);
        void Patch(int id, MediaPathType mediaPathType);
        void Delete(int id);
    }

    public class MediaPathTypeService : IMediaPathTypeService
    {
        public IEnumerable<MediaPathType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathTypes = container.MediaPathTypes
                .AsEnumerable();

            return mediaPathTypes;
        }

        public MediaPathType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathType = container.MediaPathTypes.ByKey(id)
                .GetValue();

            return mediaPathType;
        }

        public void Post(MediaPathType mediaPathType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToMediaPathTypes(mediaPathType);
            container.SaveChanges();
        }

        public void Patch(int id, MediaPathType mediaPathType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.MediaPathTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathType = container.MediaPathTypes.Where(e => e.Id == id).SingleOrDefault();
            if (mediaPathType == null) return;

            container.DeleteObject(mediaPathType);
            container.SaveChanges();
        }
    }
}
