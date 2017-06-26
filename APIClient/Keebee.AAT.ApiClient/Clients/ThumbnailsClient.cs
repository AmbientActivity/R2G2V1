using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IThumbnailsClient
    {
        IEnumerable<Thumbnail> Get();
        Thumbnail Get(Guid id);
        string Post(Thumbnail thumbnail);
        string Patch(Guid id, Thumbnail thumbnail);
        string Delete(Guid id);
    }

    public class ThumbnailsClient : BaseClient, IThumbnailsClient
    {
        public IEnumerable<Thumbnail> Get()
        {
            var request = new RestRequest("thumbnails", Method.GET);
            var data = Execute(request);
            var thumbnails = JsonConvert.DeserializeObject<IEnumerable<Thumbnail>>(data.Content);

            return thumbnails;
        }

        public Thumbnail Get(Guid id)
        {
            var request = new RestRequest($"thumbnails/{id}", Method.GET);
            var data = Execute(request);
            var thumbnail = JsonConvert.DeserializeObject<Thumbnail>(data.Content);

            return thumbnail;
        }

        public string Patch(Guid id, Thumbnail thumbnail)
        {
            var request = new RestRequest($"thumbnails/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(thumbnail);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Post(Thumbnail thumbnail)
        {
            var request = new RestRequest("thumbnails", Method.POST);
            var json = request.JsonSerializer.Serialize(thumbnail);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Delete(Guid id)
        {
            var request = new RestRequest($"thumbnails/{id}", Method.DELETE);
            var msg = Execute(request).Content;
            return (msg.Length == 0) ? null : msg;
        }
    }
}
