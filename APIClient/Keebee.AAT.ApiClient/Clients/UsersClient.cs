using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IUsersClient
    {
        IEnumerable<User> Get();
        User Get(int id);
        User GetByUsername(string username);
        int GetCount();
        int Post(User user);
        void Patch(int id, User user);
        string Delete(int id);
    }

    public class UsersClient : BaseClient, IUsersClient
    {
        public IEnumerable<User> Get()
        {
            var request = new RestRequest("users", Method.GET);
            var data = Execute(request);
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(data.Content);

            return users;
        }

        public User Get(int id)
        {
            var request = new RestRequest($"users/{id}", Method.GET);
            var data = Execute(request);
            var user = JsonConvert.DeserializeObject<User>(data.Content);

            return user;
        }

        public User GetByUsername(string username)
        {
            var request = new RestRequest($"users?username={username}", Method.GET);
            var data = Execute(request);
            var user = JsonConvert.DeserializeObject<User>(data.Content);

            return user;
        }

        public int GetCount()
        {
            var request = new RestRequest("users/count", Method.GET);
            var data = Execute(request);
            var count = Convert.ToInt32(data.Content);

            return count;
        }

        public void Patch(int id, User user)
        {
            var request = new RestRequest($"users/{id}", Method.PATCH);
            var json = request.JsonSerializer.Serialize(user);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            Execute(request);
        }

        public int Post(User user)
        {
            var request = new RestRequest("users", Method.POST);
            var json = request.JsonSerializer.Serialize(user);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"users/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
