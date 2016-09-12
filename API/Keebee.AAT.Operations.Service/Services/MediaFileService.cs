using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IMediaFileService
    {
        IEnumerable<MediaFile> Get();
        MediaFile Get(Guid streamId);
        IEnumerable<MediaFile> GetForProfile(int profileId);
        IEnumerable<MediaFile> GetForPath(string path);
        IEnumerable<MediaFile> GetForProfilePath(int profileId, string path);
    }

    public class MediaFileService : IMediaFileService
    {
        public IEnumerable<MediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AsEnumerable();

            return media;
        }

        public MediaFile Get(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles.ByKey(streamId)
                .GetValue();

            return media;
        }

        public IEnumerable<MediaFile> GetForProfile(int profileId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", $@"indexof(Path, 'Profiles\{profileId}') gt 0 and IsFolder eq false")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<MediaFile> GetForPath(string path)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", $@"indexof(Path, '{path}') gt 0 " +
                    "and IsFolder eq false")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<MediaFile> GetForProfilePath(int profileId, string path)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", $@"indexof(Path, 'Profiles\{profileId}\{path}') gt 0 " +
                    "and IsFolder eq false")
                .AsEnumerable();

            return media;
        }
    }
}