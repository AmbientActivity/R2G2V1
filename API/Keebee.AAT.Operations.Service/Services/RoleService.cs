using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IRoleService
    {
        IEnumerable<Role> Get();
        Role Get(int id);
        int Post(Role role);
        void Patch(int id, Role role);
        void Delete(int id);
    }

    public class RoleService : IRoleService
    {
        public IEnumerable<Role> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var roles = container.Roles.AsEnumerable();

            return roles;
        }

        public Role Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var role = container.Roles.ByKey(id).GetValue();

            return role;
        }

        public int Post(Role role)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToRoles(role);
            container.SaveChanges();

            var roleId = role.Id;

            return roleId;
        }

        public void Patch(int id, Role role)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.Roles.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (role.Description != null)
                el.Description = role.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var role = container.Roles.Where(e => e.Id == id).SingleOrDefault();
            if (role == null) return;

            container.DeleteObject(role);
            container.SaveChanges();
        }
    }
}