using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class UserRole
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }

    public class UserRoleList
    {
        public IEnumerable<UserRole> UserRoles;
    }


    public class UserRoleSingle
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IEnumerable<Role> Roles;
    }

    public class UserRolesList
    {
        public IEnumerable<UserRoleSingle> UserRoles;
    }
}
