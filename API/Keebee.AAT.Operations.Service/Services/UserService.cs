using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IUserService
    {
        IEnumerable<User> Get();
        User Get(int id);
        User GetByUsername(string username);
        int GetCount();  // for the KeepIISAlive Service
        int Post(User user);
        void Patch(int id, User user);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        public IEnumerable<User> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var users = container.Users.AsEnumerable();

            return users;
        }

        public User Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var user = container.Users.ByKey(id);

            User result;
            try { result = user.GetValue(); }
            catch { result = null; }

            return result;
        }

        public User GetByUsername(string username)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var user = container.Users.AddQueryOption("$filter",$"Username eq '{username}'")
                .FirstOrDefault();

            return user;
        }

        public int GetCount()
        {
            var container = new Container(new Uri(ODataHost.Url));
            return container.Users.Count();
        }

        public int Post(User user)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToUsers(user);
            container.SaveChanges();

            var userId = user.Id;

            return userId;
        }

        public void Patch(int id, User user)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.Users.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (user.Username != null)
                el.Username = user.Username;

            if (user.Password != null)
                el.Password = user.Password;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var user = container.Users.Where(e => e.Id == id).SingleOrDefault();
            if (user == null) return;

            container.DeleteObject(user);
            container.SaveChanges();
        }
    }
}