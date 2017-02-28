using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IMediaFilesClient
    {
        MediaFileSingle Get(Guid streamId);
        IEnumerable<Media> GetForPath(string path);
        MediaFileSingle GetFromPath(string path, string filename);
        byte[] GetFileBytes(Guid streamId);
        byte[] GetFileStreamFromPath(string path, string filename);
    }

    public class MediaFilesClient : BaseClient, IMediaFilesClient
    {
        public MediaFileSingle Get(Guid streamId)
        {
            var request = new RestRequest($"mediafiles/{streamId}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<MediaFileSingle>(data.Content);

            return mediaFile;
        }

        public IEnumerable<Media> GetForPath(string path)
        {
            var request = new RestRequest($"mediafiles?path={path}", Method.GET);
            var data = Execute(request);
            var media = JsonConvert.DeserializeObject<MediaList>(data.Content).Media;

            return media;
        }

        public MediaFileSingle GetFromPath(string path, string filename)
        {
            var request = new RestRequest($"mediafiles?path={path}&filename={filename}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<MediaFileSingle>(data.Content);

            return mediaFile;
        }

        public byte[] GetFileBytes(Guid streamId)
        {
            var request = new RestRequest($"mediafilestreams/{streamId}", Method.GET);
            var data = Execute(request);

            return data.RawBytes;
        }

        public byte[] GetFileStreamFromPath(string path, string filename)
        {
            var request = new RestRequest($"mediafilestreams?path={path}&filename={filename}", Method.GET);
            var data = Execute(request);
            var bytes = JsonConvert.DeserializeObject<MediaFileStreamSingle>(data.Content).Stream;

            return bytes;
        }
    }
}
