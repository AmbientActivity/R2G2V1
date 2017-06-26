using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using Keebee.AAT.ThumbnailGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IThumbnailService
    {
        IEnumerable<Thumbnail> Get();
        Thumbnail Get(Guid id);
        byte[] GetImage(Guid id);
        void Post(Thumbnail thumbnail);
        void Patch(Guid id, Thumbnail thumbnail);
        void Delete(Guid id);
    }

    public class ThumbnailService : IThumbnailService
    {
        public IEnumerable<Thumbnail> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var thumbnails = container.Thumbnails.AsEnumerable();

            return thumbnails;
        }

        public Thumbnail Get(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));
            Thumbnail thumbnail;

            try
            {
                thumbnail = container.Thumbnails.ByKey(streamId).GetValue();
            }
            catch
            {
                return null;
            }
            
            return thumbnail;
        }

        public byte[] GetImage(Guid streamId)
        {
            try
            {
                byte[] byteArray;

                var thumbnail = Get(streamId);
                if (thumbnail == null)
                {
                    var thumbnailGenerator = new ThumbnailGenerator();

                    string errorMessage;
                    byteArray = thumbnailGenerator.GetAndSave(streamId, false, out errorMessage);
                    if (errorMessage != null) throw new Exception(errorMessage);
                }
                else
                {
                    byteArray = thumbnail.Image;
                }

                return byteArray;
            }

            catch(Exception ex)
            {
                // do something with the message
                var message = ex.Message;

                return null;
            }
        }

        public void Post(Thumbnail thumbnail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToThumbnails(thumbnail);
            container.SaveChanges();
        }

        public void Patch(Guid id, Thumbnail thumbnail)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.Thumbnails.Where(e => e.StreamId == id).SingleOrDefault();
            if (el == null) return;

            if (thumbnail.Image != null)
                el.Image = thumbnail.Image;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var thumbnail = container.Thumbnails.Where(e => e.StreamId == id).SingleOrDefault();
            if (thumbnail == null) return;

            container.DeleteObject(thumbnail);
            container.SaveChanges();
        }
    }
}