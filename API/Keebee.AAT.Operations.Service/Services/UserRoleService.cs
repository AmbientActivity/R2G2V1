using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IUserRoleService
    {
        IEnumerable<UserRole> Get();
        UserRole Get(int id);
        IEnumerable<UserRole> GetByUser(int userId);
        int Post(UserRole userRole);
        void Patch(int id, UserRole userRole);
        void Delete(int id);
    }

    public class UserRoleService : IUserRoleService
    {
        public IEnumerable<UserRole> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var userRoles = container.UserRoles
                .Expand("User,Role")
                .AsEnumerable();

            return userRoles;
        }

        public UserRole Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var userRole = container.UserRoles.ByKey(id)
                .Expand("User,Role");

            UserRole result;
            try { result = userRole.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<UserRole> GetByUser(int userId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var userRoles = container.UserRoles
                .AddQueryOption("$filter", $"UserId eq {userId}")
                .Expand("User,Role")
                .AsEnumerable();

            return userRoles;
        }

        public int Post(UserRole userRole)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToUserRoles(userRole);
            container.SaveChanges();

            var userRoleId = userRole.Id;

            return userRoleId;
        }

        public void Patch(int id, UserRole userRole)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.UserRoles.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (userRole.UserId > 0)
                el.UserId = userRole.UserId;

            if (userRole.RoleId > 0)
                el.RoleId = userRole.RoleId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var userRole = container.UserRoles.Where(e => e.Id == id).SingleOrDefault();
            if (userRole == null) return;

            container.DeleteObject(userRole);
            container.SaveChanges();
        }
    }
}