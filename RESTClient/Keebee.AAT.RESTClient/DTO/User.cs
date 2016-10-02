using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserList
    {
        public IEnumerable<User> Users;
    }
}
