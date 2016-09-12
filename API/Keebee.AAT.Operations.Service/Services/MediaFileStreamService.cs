using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IMediaFileStreamService
    {
        MediaFileStream Get(Guid streamId);
        IEnumerable<MediaFileStream> GetForPath(string path);
    }

    public class MediaFileStreamService : IMediaFileStreamService
    {
        public MediaFileStream Get(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFileStreams.ByKey(streamId)
                .GetValue();

            return media;
        }

        public IEnumerable<MediaFileStream> GetForPath(string path)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFileStreams
                .AsEnumerable();

            return media;
        }
    }
}