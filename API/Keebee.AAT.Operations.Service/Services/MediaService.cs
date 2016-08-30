using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IMediaService
    {
        IEnumerable<MediaFile> Get();
        MediaFile Get(Guid streamId);
        void Post(MediaFile media);
        void Patch(Guid id, MediaFile media);
        void Delete(Guid id);
    }

    public class MediaService : IMediaService
    {
        public IEnumerable<MediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var medias = container.MediaFiles
                .AsEnumerable();

            return medias;
        }

        public MediaFile Get(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles.ByKey(streamId)
                .GetValue();

            return media;
        }

        public void Post(MediaFile media)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToMediaFiles(media);
            container.SaveChanges();
        }

        public void Patch(Guid id, MediaFile media)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var p = container.MediaFiles.Where(e => e.StreamId == id).SingleOrDefault();
            if (p == null) return;
            container.UpdateObject(p);
            container.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles.Where(e => e.StreamId == id).SingleOrDefault();
            if (media == null) return;

            container.DeleteObject(media);
            container.SaveChanges();
        }
    }
}