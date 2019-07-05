using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IPublicMediaFilesClient
    {
        IEnumerable<ResponseTypePaths> Get();
        PublicMediaFile Get(int id);
        IEnumerable<ResponseTypePaths> Get(bool isSystem);
        IEnumerable<MediaPathTypeFiles> GetForResponseType(int responseTypeId);
        IEnumerable<ResponseTypePaths> GetForMediaPathType(int mediaPathTypeId);
        PublicMediaFile GetForResponseTypeFilename(int responseTypeId, string filename);
        IEnumerable<PublicMediaFile> GetForStreamId(Guid streamId);
        IEnumerable<ResponseTypePaths> GetLinkedForStreamId(Guid streamId);
        int[] GetIdsForStreamId(Guid streamId);
        IEnumerable<ResponseTypePaths> GetLinked();
        string Patch(int id, PublicMediaFileEdit publicMediaFile);
        string Post(PublicMediaFileEdit publicMediaFile, out int newId);
        string Delete(int id);
    }

    public class PublicMediaFilesClient : BaseClient, IPublicMediaFilesClient
    {
        public PublicMediaFile Get(int id)
        {
            var request = new RestRequest($"publicmediafiles/{id}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<PublicMediaFile>(data.Content);

            return mediaFile;
        }

        public IEnumerable<ResponseTypePaths> Get()
        {
            var request = new RestRequest("publicmediafiles", Method.GET);
            var data = Execute(request);
            var mediaResponseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseTypePaths>>(data.Content);

            return mediaResponseTypes;
        }

        public IEnumerable<ResponseTypePaths> Get(bool isSystem)
        {
            var request = new RestRequest($"publicmediafiles?isSystem={isSystem}", Method.GET);
            var data = Execute(request);
            var mediaResponseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseTypePaths>>(data.Content);

            return mediaResponseTypes;
        }

        public IEnumerable<MediaPathTypeFiles> GetForResponseType(int responseTypeId)
        {
            var request = new RestRequest($"publicmediafiles?responseTypeId={responseTypeId}", Method.GET);
            var data = Execute(request);
            var mediaPathTypeFiles = JsonConvert.DeserializeObject<IEnumerable<MediaPathTypeFiles>>(data.Content);

            return mediaPathTypeFiles;
        }

        public IEnumerable<ResponseTypePaths> GetForMediaPathType(int mediaPathTypeId)
        {
            var request = new RestRequest($"publicmediafiles?mediaPathTypeId={mediaPathTypeId}", Method.GET);
            var data = Execute(request);
            var mediaResponseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseTypePaths>>(data.Content);

            return mediaResponseTypes;
        }

        public IEnumerable<PublicMediaFile> GetForStreamId(Guid streamId)
        {
            var request = new RestRequest($"publicmediafiles?streamId={streamId}", Method.GET);
            var data = Execute(request);
            var mediaResponseTypes = JsonConvert.DeserializeObject<IEnumerable<PublicMediaFile>>(data.Content);

            return mediaResponseTypes;
        }

        public PublicMediaFile GetForResponseTypeFilename(int responseTypeId, string filename)
        {
            var request = new RestRequest($"publicmediafiles?responseTypeId={responseTypeId}&filename={filename}", Method.GET);
            var data = Execute(request);
            var mediaFile = JsonConvert.DeserializeObject<PublicMediaFile>(data.Content);

            return mediaFile;
        }

        public IEnumerable<ResponseTypePaths> GetLinked()
        {
            var request = new RestRequest("publicmediafiles/linked", Method.GET);
            var data = Execute(request);
            var mediaResponseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseTypePaths>>(data.Content);

            return mediaResponseTypes;
        }

        public IEnumerable<ResponseTypePaths> GetLinkedForStreamId(Guid streamId)
        {
            var request = new RestRequest($"publicmediafiles/linked?streamId={streamId}", Method.GET);
            var data = Execute(request);
            var mediaResponseTypes = JsonConvert.DeserializeObject<IEnumerable<ResponseTypePaths>>(data.Content);

            return mediaResponseTypes;
        }

        public int[] GetIdsForStreamId(Guid streamId)
        {
            var request = new RestRequest($"publicmediafiles/ids?streamId={streamId}", Method.GET);
            var data = Execute(request);
            var ids = JsonConvert.DeserializeObject<int[]>(data.Content);

            return ids;
        }

        public string Patch(int id, PublicMediaFileEdit mediaFile)
        {
            var request = new RestRequest($"publicmediafiles/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(mediaFile);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = Execute(request);

            string msg = null;
            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Post(PublicMediaFileEdit publicMediaFile, out int newId)
        {
            var request = new RestRequest("publicmediafiles", Method.POST);
            var json = request.JsonSerializer.Serialize(publicMediaFile);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = Execute(request);

            string result = null;
            newId = -1;

            if (response.StatusCode == HttpStatusCode.OK)
                newId = Convert.ToInt32(response.Content);
            else
                result = response.StatusDescription;

            return result;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"publicmediafiles/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
