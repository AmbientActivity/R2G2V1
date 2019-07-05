using System;
using System.Collections.Generic;
using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IMediaFileStreamsClient
    {
        //IEnumerable<MediaFileStream> Get();
        MediaFileStream Get(Guid streamId);
        MediaFileStream GetFromPath(string path, string filename);
    }

    public class MediaFileStreamsClient : BaseClient, IMediaFileStreamsClient
    {
        //public IEnumerable<MediaFileStream> Get()
        //{
        //    var request = new RestRequest("mediafilestreams", Method.GET);
        //    var data = Execute(request);
        //    var mediaFiles = JsonConvert.DeserializeObject<IEnumerable<MediaFileStream>>(data.Content);

        //    return mediaFiles;
        //}

        public MediaFileStream Get(Guid streamId)
        {
            var request = new RestRequest($"mediafilestreams/{streamId}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<MediaFileStream>(data.Content);

            return mediaFile;
        }

        public MediaFileStream GetFromPath(string path, string filename)
        {
            var request = new RestRequest($"mediafilestreams?path={path}&filename={filename}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<MediaFileStream>(data.Content);

            return mediaFile;
        }
    }
}
