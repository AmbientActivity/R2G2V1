using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResidentMediaFilesClient
    {
        IEnumerable<ResidentMedia> Get();
        ResidentMediaFile Get(int id);
        ResidentMedia GetForResident(int residentId);
        IEnumerable<MediaPathTypeFiles> GetForResidentResponseType(int residentId, int responseTypeId);
        IEnumerable<ResidentMedia> GetLinked();
        IEnumerable<ResidentMedia> GetLinkedForStreamId(Guid streamId);
        int[] GetIdsForStreamId(Guid streamId);
        int Post(ResidentMediaFileEdit residenttMediaFile);
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

        public ResidentMedia GetForResident(int residentId)
        {
            var request = new RestRequest($"residentmediafiles?residentId={residentId}", Method.GET);
            var data = Execute(request);
            var residentMedia = JsonConvert.DeserializeObject<ResidentMedia>(data.Content);

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

        public int Post(ResidentMediaFileEdit residenttMediaFile)
        {
            var request = new RestRequest("residentmediafiles", Method.POST);
            var json = request.JsonSerializer.Serialize(residenttMediaFile);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"residentmediafiles/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
