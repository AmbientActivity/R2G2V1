using System;
using System.Collections.Generic;
using System.Net;
using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IAmbientInvitationsClient
    {
        IEnumerable<AmbientInvitation> Get();
        AmbientInvitation Get(int id);
        string Post(AmbientInvitationEdit invitation, out int newId);
        string Patch(int id, AmbientInvitationEdit invitation);
        string Delete(int id);
    }

    public class AmbientInvitationsClient : BaseClient, IAmbientInvitationsClient
    {
        public IEnumerable<AmbientInvitation> Get()
        {
            var request = new RestRequest("ambientinvitations", Method.GET);
            var data = Execute(request);
            var ambientInvitations = JsonConvert.DeserializeObject<IEnumerable<AmbientInvitation>>(data.Content);

            return ambientInvitations;
        }

        public AmbientInvitation Get(int id)
        {
            var request = new RestRequest("ambientinvitations/1", Method.GET);
            var data = Execute(request);
            var ambientInvitation = JsonConvert.DeserializeObject<AmbientInvitation>(data.Content);

            return ambientInvitation;
        }

        public string Post(AmbientInvitationEdit invitation, out int newId)
        {
            var request = new RestRequest("ambientinvitations", Method.POST);
            var json = request.JsonSerializer.Serialize(invitation);
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

        public string Patch(int id, AmbientInvitationEdit invitation)
        {
            var request = new RestRequest($"ambientinvitations/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(invitation);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = Execute(request);

            string msg = null;

            if (response.StatusCode != HttpStatusCode.NoContent)
                msg = response.StatusDescription;

            return msg;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"ambientinvitations/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
