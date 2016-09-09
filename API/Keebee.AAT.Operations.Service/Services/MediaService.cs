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
    }
}