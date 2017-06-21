using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IMediaFilesClient
    {
        MediaFilePath Get(Guid streamId);
        IEnumerable<Media> GetForPath(string path);
        MediaFilePath GetFromPath(string path, string filename);
        byte[] GetStream(Guid streamId);
        byte[] GetFileStreamFromPath(string path, string filename);
        IEnumerable<Media> GetWithLinkedData(string path);
    }

    public class MediaFilesClient : BaseClient, IMediaFilesClient
    {
        public MediaFilePath Get(Guid streamId)
        {
            var request = new RestRequest($"mediafiles/{streamId}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<MediaFilePath>(data.Content);

            return mediaFile;
        }

        public IEnumerable<Media> GetForPath(string path)
        {
            var request = new RestRequest($"mediafiles?path={path}", Method.GET);
            var data = Execute(request);
            var media = JsonConvert.DeserializeObject<IEnumerable<Media>>(data.Content);

            return media;
        }

        public MediaFilePath GetFromPath(string path, string filename)
        {
            var request = new RestRequest($"mediafiles?path={path}&filename={filename}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<MediaFilePath>(data.Content);

            return mediaFile;
        }

        public byte[] GetStream(Guid streamId)
        {
            var request = new RestRequest($"mediafilestreams/{streamId}", Method.GET);
            var data = Execute(request);

            return data.RawBytes;
        }

        public byte[] GetFileStreamFromPath(string path, string filename)
        {
            var request = new RestRequest($"mediafilestreams?path={path}&filename={filename}", Method.GET);
            var data = Execute(request);
            var bytes = JsonConvert.DeserializeObject<MediaFileStream>(data.Content).Stream;

            return bytes;
        }

        public IEnumerable<Media> GetWithLinkedData(string path)
        {
            var request = new RestRequest($"mediafiles/linked?path={path}", Method.GET);
            var data = Execute(request);
            var media = JsonConvert.DeserializeObject<IEnumerable<Media>>(data.Content);

            return media;
        }
    }
}
