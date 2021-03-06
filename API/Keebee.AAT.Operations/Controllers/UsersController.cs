﻿using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
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
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<User> users = new Collection<User>();

            await Task.Run(() =>
            {
                users = _userService.Get();
            });

            if (users == null) return new DynamicJsonArray(new object[0]);

            var jArray = users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Password
                }).ToArray();

            return new DynamicJsonArray(jArray);
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

        // GET: api/Users/Count
        [HttpGet]
        [Route("count")]
        public async Task<int> Count()
        {
            var count = 0;

            await Task.Run(() =>
            {
                count = _userService.GetCount();
            });

            return count;
        }

        // POST: api/Users
        [HttpPost]
        public int Post([FromBody]User user)
        {
            return _userService.Post(user);
        }

        // PATCH: api/Users/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]User user)
        {
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
