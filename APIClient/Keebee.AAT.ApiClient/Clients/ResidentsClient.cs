using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
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
        void Patch(int residentId, ResidentEdit resident);
        int Post(ResidentEdit resident);
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

        public void Patch(int id, ResidentEdit resident)
        {
            var request = new RestRequest($"residents/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(resident);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            Execute(request);
        }

        public int Post(ResidentEdit resident)
        {
            var request = new RestRequest("residents", Method.POST);
            var json = request.JsonSerializer.Serialize(resident);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"residents/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
