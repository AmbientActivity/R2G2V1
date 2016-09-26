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
        MediaFileStream GetSingleFromPath(string path, string filename);
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

        public MediaFileStream GetSingleFromPath(string path, string filename)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var fileStream = container.MediaFileStreams
                .AddQueryOption("$filter", $@"indexof(Path, '{path}') gt 0 " +
                    $"and Filename eq '{filename}'");

            return fileStream?.Single();
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