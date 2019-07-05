using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResidentsClient
    {
        IEnumerable<Resident> Get();
        Resident Get(int residentId);
        Resident GetWithMedia(int residentId);
        Resident GetByNameGender(string firstName, string lastName, string gender);
        bool Exists(int residentId);
        string Patch(int residentId, Resident resident);
        string Post(Resident resident, out int newId);
    }

    public class ResidentsClient : BaseClient, IResidentsClient
    {
        public IEnumerable<Resident> Get()
        {
            var request = new RestRequest("residents", Method.GET);
            var data = Execute(request);
            var residents = JsonConvert.DeserializeObject<IEnumerable<Resident>>(data.Content);

            return residents;
        }

        public Resident Get(int residentId)
        {
            var request = new RestRequest($"residents/{residentId}", Method.GET);
            var data = Execute(request);
            var resident = JsonConvert.DeserializeObject<Resident>(data.Content);

            return resident;
        }

        public Resident GetByNameGender(string firstName, string lastName, string gender)
        {
            var request = new RestRequest($"residents?firstName={firstName}&lastName={lastName}&gender={gender}", Method.GET);
            var data = Execute(request);
            var resident = JsonConvert.DeserializeObject<Resident>(data.Content);

            return resident;
        }

        public Resident GetWithMedia(int residentId)
        {
            var request = new RestRequest($"residents/{residentId}/media", Method.GET);
            var data = Execute(request);
            var resident = JsonConvert.DeserializeObject<Resident>(data.Content);

            return resident;
        }

        public bool Exists(int residentId)
        {
            var request = new RestRequest($"residents/{residentId}/exists", Method.GET);
            var data = Execute(request);

            return Convert.ToBoolean(data.Content);
        }

        public string Patch(int id, Resident resident)
        {
            var request = new RestRequest($"residents/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(resident);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Post(Resident resident, out int newId)
        {
            var request = new RestRequest("residents", Method.POST);
            var json = request.JsonSerializer.Serialize(resident);
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
            var request = new RestRequest($"residents/{id}", Method.DELETE);
            var response = Execute(request);
            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }
    }
}
