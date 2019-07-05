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
        IEnumerable<MediaFile> GetForResident(int residentId);
        IEnumerable<MediaFile> GetForPath(string path);
        IEnumerable<MediaFile> GetForResidentPath(int profileId, string path);
        MediaFile GetSingleFromPath(string path, string filename);
    }

    public class MediaFileService : IMediaFileService
    {
        public IEnumerable<MediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", "IsFolder eq false")
                .AsEnumerable();

            return media;
        }

        public MediaFile Get(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles.ByKey(streamId);

            MediaFile result;
            try { result = media.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<MediaFile> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", $@"indexof(Path, 'Profiles\{residentId}') gt 0 and IsFolder eq false")
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

        public MediaFile GetSingleFromPath(string path, string filename)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", $@"indexof(Path, '{path}') gt 0 " +
                    $"and Filename eq '{filename.Replace("'", "''").Replace("&", "%26")}'")
                    .ToList();

            return media.Any() ? media.Single() : null;
        }

        public IEnumerable<MediaFile> GetForResidentPath(int residentId, string path)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.MediaFiles
                .AddQueryOption("$filter", $@"indexof(Path, 'Profiles\{residentId}\{path}') gt 0 " +
                    "and IsFolder eq false")
                .AsEnumerable();

            return media;
        }
    }
}