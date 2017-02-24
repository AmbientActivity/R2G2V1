using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IUserRolesClient
    {
        IEnumerable<UserRole> Get();
        UserRole Get(int userRoleId);
        IEnumerable<UserRoleSingle> GetByUser(int userId);
    }

    public class UserRolesClient
    {
        private readonly ClientBase _clientBase;

        public UserRolesClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<UserRole> Get()
        {
            var data = _clientBase.Get("userroles");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var userroles = serializer.Deserialize<UserRoleList>(data).UserRoles.ToList();

            return userroles;
        }

        public UserRole GetUserRole(int userRoleId)
        {
            var data = _clientBase.Get($"userroles/{userRoleId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var userRole = serializer.Deserialize<UserRole>(data);

            return userRole;
        }

        public IEnumerable<UserRoleSingle> GetByUser(int userId)
        {
            var data = _clientBase.Get($"userroles?userId={userId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var userRoles = serializer.Deserialize<UserRolesList>(data).UserRoles;

            return userRoles;
        }
    }
}
