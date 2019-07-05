using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<Role> roles = new Collection<Role>();

            await Task.Run(() =>
            {
                roles = _roleService.Get();
            });

            if (roles == null) return new DynamicJsonArray(new object[0]);

            var jArray = roles
                .Select(u => new
                {
                    u.Id,
                    u.Description,
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/Roles/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var role = new Role();

            await Task.Run(() =>
            {
                role = _roleService.Get(id);
            });

            if (role == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = role.Id;
            exObj.Rolename = role.Description;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/Roles
        [HttpPost]
        public int Post([FromBody]Role role)
        {
            return _roleService.Post(role);
        }

        // PUT: api/Roles/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]Role role)
        {
            _roleService.Patch(id, role);
        }

        // DELETE: api/Roles/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _roleService.Delete(id);
        }
    }
}
