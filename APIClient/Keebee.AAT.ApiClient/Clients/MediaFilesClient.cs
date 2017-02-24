using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IMediaFilesClient
    {
        MediaFileSingle Get(Guid streamId);
        IEnumerable<Media> GetForPath(string path);
        MediaFileSingle GetFromPath(string path, string filename);
        byte[] GetFileStream(Guid streamId);
        byte[] GetFileStreamFromPath(string path, string filename);
    }

    public class MediaFilesClient : IMediaFilesClient
    {
        private readonly ClientBase _clientBase;

        public MediaFilesClient()
        {
            _clientBase = new ClientBase();
        }

        public MediaFileSingle Get(Guid streamId)
        {
            var data = _clientBase.Get($"mediafiles/{streamId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaFile = serializer.Deserialize<MediaFileSingle>(data);

            return mediaFile;
        }

        public IEnumerable<Media> GetForPath(string path)
        {
            var data = _clientBase.Get($"mediafiles?path={path}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaList = serializer.Deserialize<MediaList>(data).Media;

            return mediaList;
        }

        public MediaFileSingle GetFromPath(string path, string filename)
        {
            var data = _clientBase.Get($"mediafiles?path={path}&filename={filename}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var media = serializer.Deserialize<MediaFileSingle>(data);

            return media;
        }

        public byte[] GetFileStream(Guid streamId)
        {
            var data = _clientBase.GetBytes($"mediafilestreams/{streamId}");

            return data;
        }

        public byte[] GetFileStreamFromPath(string path, string filename)
        {
            var data = _clientBase.Get($"mediafilestreams?path={path}&filename={filename}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var media = serializer.Deserialize<MediaFileStreamSingle>(data).Stream;

            return media;
        }
    }
}
