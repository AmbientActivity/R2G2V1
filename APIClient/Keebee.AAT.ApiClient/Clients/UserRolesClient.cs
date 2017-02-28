using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IUserRolesClient
    {
        IEnumerable<UserRole> Get();
        UserRole Get(int userRoleId);
        IEnumerable<UserRoleSingle> GetByUser(int userId);
    }

    public class UserRolesClient : BaseClient, IUserRolesClient
    {
        public IEnumerable<UserRole> Get()
        {
            var request = new RestRequest("userroles", Method.GET);
            var data = Execute(request);
            var userRoles = JsonConvert.DeserializeObject<UserRoleList>(data.Content).UserRoles;

            return userRoles;
        }

        public UserRole Get(int userRoleId)
        {
            var request = new RestRequest($"userroles/{userRoleId}", Method.GET);
            var data = Execute(request);
            var userRole = JsonConvert.DeserializeObject<UserRole>(data.Content);

            return userRole;
        }

        public IEnumerable<UserRoleSingle> GetByUser(int userId)
        {
            var request = new RestRequest($"userroles?userId={userId}", Method.GET);
            var data = Execute(request);
            var userRoles = JsonConvert.DeserializeObject<UserRolesList>(data.Content).UserRoles;

            return userRoles;
        }
    }
}
