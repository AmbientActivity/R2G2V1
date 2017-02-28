using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/userroles")]
    public class UserRolesController : ApiController
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        // GET: api/UserRoles
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<UserRole> userRoles = new Collection<UserRole>();

            await Task.Run(() =>
            {
                userRoles = _userRoleService.Get();
            });

            if (userRoles == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.UserRoles = userRoles
                .GroupBy(ur => ur.User)
                .Select(roles => new { roles.First().User, Roles = roles })
                .Select(u => new
                {
                    u.User.Id,
                    u.User.Username,
                    Roles = u.Roles.Select(r => new
                    {
                        r.Role.Id,
                        r.Role.Description
                    })
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/UserRoles/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var userRole = new UserRole();

            await Task.Run(() =>
            {
                userRole = _userRoleService.Get(id);
            });

            if (userRole == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = userRole.Id;
            exObj.User = new
            {
                userRole.User.Id,
                userRole.User.Username
            };
            exObj.Role = new
            {
                userRole.Role.Id,
                userRole.Role.Description
            };

            return new DynamicJsonObject(exObj);
        }

        // GET: api/UserRoles?userid=1
        [HttpGet]
        public async Task<DynamicJsonObject> GetByUser(int userId)
        {
            IEnumerable<UserRole> userRoles = new Collection<UserRole>();

            await Task.Run(() =>
            {
                userRoles = _userRoleService.GetByUser(userId);
            });

            if (userRoles == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.UserRoles = userRoles
                .GroupBy(ur => ur.User)
                .Select(roles => new { roles.First().User, Roles = roles })
                .Select(u => new
                {
                    u.User.Id,
                    u.User.Username,
                    Roles = u.Roles.Select(r => new
                    {
                        r.Role.Id,
                        r.Role.Description
                    })
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/UserRoles
        [HttpPost]
        public int Post([FromBody]UserRole userRole)
        {
            return _userRoleService.Post(userRole);
        }

        // PUT: api/UserRoles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]UserRole userRole)
        {
            _userRoleService.Patch(id, userRole);
        }

        // DELETE: api/UserRoles/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _userRoleService.Delete(id);
        }
    }
}
