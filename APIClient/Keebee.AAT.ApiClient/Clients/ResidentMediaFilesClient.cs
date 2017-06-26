using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResidentMediaFilesClient
    {
        IEnumerable<ResidentMedia> Get();
        ResidentMediaFile Get(int id);
        IEnumerable<ResponseTypePaths> GetForResident(int residentId);
        IEnumerable<MediaPathTypeFiles> GetForResidentResponseType(int residentId, int responseTypeId);
        IEnumerable<ResidentMedia> GetLinked();
        IEnumerable<ResidentMedia> GetLinkedForStreamId(Guid streamId);
        int[] GetIdsForStreamId(Guid streamId);
        string Patch(int id, ResidentMediaFileEdit residenttMediaFile);
        string Post(ResidentMediaFileEdit residenttMediaFile, out int newId);
        string Delete(int id);
    }

    public class ResidentMediaFilesClient : BaseClient, IResidentMediaFilesClient
    {
        public IEnumerable<ResidentMedia> Get()
        {
            var request = new RestRequest("residentmediafiles", Method.GET);
            var data = Execute(request);
            var residentMedia = JsonConvert.DeserializeObject<IEnumerable<ResidentMedia>>(data.Content);

            return residentMedia;
        }

        public ResidentMediaFile Get(int id)
        {
            var request = new RestRequest($"residentmediafiles/{id}", Method.GET);
            var data = Execute(request);
            var residentMediaFile = JsonConvert.DeserializeObject<ResidentMediaFile>(data.Content);

            return residentMediaFile;
        }

        public IEnumerable<ResponseTypePaths> GetForResident(int residentId)
        {
            var request = new RestRequest($"residentmediafiles?residentId={residentId}", Method.GET);
            var data = Execute(request);
            var residentMedia = JsonConvert.DeserializeObject<IEnumerable<ResponseTypePaths>>(data.Content);

            return residentMedia;
        }

        public IEnumerable<MediaPathTypeFiles> GetForResidentResponseType(int residentId, int responseTypeId)
        {
            var request = new RestRequest($"residentmediafiles?residentId={residentId}&responseTypeId={responseTypeId}", Method.GET);
            var data = Execute(request);
            var mediaPathTypeFiles = JsonConvert.DeserializeObject<IEnumerable<MediaPathTypeFiles>>(data.Content);

            return mediaPathTypeFiles;
        }

        public IEnumerable<ResidentMedia> GetLinked()
        {
            var request = new RestRequest("residentmediafiles/linked", Method.GET);
            var data = Execute(request);
            var mediaList = JsonConvert.DeserializeObject<IEnumerable<ResidentMedia>>(data.Content);

            return mediaList;
        }

        public IEnumerable<ResidentMedia> GetLinkedForStreamId(Guid streamId)
        {
            var request = new RestRequest($"residentmediafiles/linked?streamId={streamId}", Method.GET);
            var data = Execute(request);
            var mediaList = JsonConvert.DeserializeObject<IEnumerable<ResidentMedia>>(data.Content);

            return mediaList;
        }

        public int[] GetIdsForStreamId(Guid streamId)
        {
            var request = new RestRequest($"residentmediafiles/ids?streamId={streamId}", Method.GET);
            var data = Execute(request);
            var ids = JsonConvert.DeserializeObject<int[]>(data.Content);

            return ids;
        }

        public string Patch(int id, ResidentMediaFileEdit mediaFile)
        {
            var request = new RestRequest($"residentmediafiles/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(mediaFile);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);
            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Post(ResidentMediaFileEdit residenttMediaFile, out int newId)
        {
            var request = new RestRequest("residentmediafiles", Method.POST);
            var json = request.JsonSerializer.Serialize(residenttMediaFile);
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
            var request = new RestRequest($"residentmediafiles/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
