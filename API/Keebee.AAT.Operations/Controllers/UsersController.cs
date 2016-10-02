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
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<User> users = new Collection<User>();

            await Task.Run(() =>
            {
                users = _userService.Get();
            });

            if (users == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Users = users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Password
            }).OrderBy(o => o.Username);

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Users/5
        [Route("{id}")]
        [HttpGet]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var user = new User();

            await Task.Run(() =>
            {
                user = _userService.Get(id);
            });

            if (user == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = user.Id;
            exObj.Username = user.Username;
            exObj.Password = user.Password;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/Users?username='charlie'
        [HttpGet]
        public async Task<DynamicJsonObject> GetByUsername(string username)
        {
            var user = new User();

            await Task.Run(() =>
            {
                user = _userService.GetByUsername(username);
            });

            if (user == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = user.Id;
            exObj.Username = user.Username;
            exObj.Password = user.Password;

            return new DynamicJsonObject(exObj);
        }
        // POST: api/Users
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var user = serializer.Deserialize<User>(value);

            return _userService.Post(user);
        }

        // PUT: api/Users/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var user = serializer.Deserialize<User>(value);
            _userService.Patch(id, user);
        }

        // DELETE: api/Users/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _userService.Delete(id);
        }
    }
}
