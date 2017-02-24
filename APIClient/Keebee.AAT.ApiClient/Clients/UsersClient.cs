using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IUsersClient
    {
        IEnumerable<User> Get();
        User Get(int id);
        User GetByUsername(string username);
        int Post(User user);
        void Patch(int id, User user);
        string Delete(int id);
    }

    public class UsersClient : IUsersClient
    {
        private readonly ClientBase _clientBase;

        public UsersClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<User> Get()
        {
            var data = _clientBase.Get("users");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var users = serializer.Deserialize<UserList>(data).Users.ToList();

            return users;
        }

        public User Get(int id)
        {
            var data = _clientBase.Get($"users/{id}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var user = serializer.Deserialize<User>(data);

            return user;
        }

        public User GetByUsername(string username)
        {
            var data = _clientBase.Get($"users?username={username}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var user = serializer.Deserialize<User>(data);

            return user;
        }

        public int Post(User user)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(user);

            return _clientBase.Post("users", el);
        }

        public void Patch(int id, User user)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(user);

            _clientBase.Patch(string.Format($"users/{id}"), el);
        }

        public string Delete(int id)
        {
            return _clientBase.Delete($"users/{id}");
        }
    }
}
