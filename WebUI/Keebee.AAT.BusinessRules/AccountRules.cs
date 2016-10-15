using Keebee.AAT.BusinessRules.DTO;
using Keebee.AAT.RESTClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Keebee.AAT.BusinessRules
{
    public class AccountRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        public int CreateUser(UserModel user)
        {
            var u = new User
            {
                Id = user.Id,
                Username = user.Username,
                Password = GeneratePasswordHash(user.Password)
            };

            var id = _opsClient.PostUser(u);

            return id;
        }

        public int AttemptToLogin(string username, string password, out string errmsg)
        {
            var user = _opsClient.GetUserByUsername(username);
            errmsg = null;

            if (user.Id == 0)
                errmsg = "User does not exist";

            if (errmsg == null)
            {
                if (password == null)
                    errmsg = "Password is required";
            }

            if (errmsg == null)
            {
                if (!VerifyHashPassword(password, user.Password.Trim()))
                    errmsg = "Invalid password";
            }

            return (errmsg == null) ? user.Id : 0;
        }

        public string GetUserRoles(int userId)
        {
            var user = _opsClient.GetRolesByUser(userId).Single();
            var s = new StringBuilder();

            foreach (var r in user.Roles)
            {
                s.Append(r.Description + "|");
            }

            return s.ToString().Substring(0, s.ToString().Length - 1);
        }

        public string ValidatePasswordChange(string username, string oldPassword, string newPassword, string retypeNewPassword, out int userId)
        {
            string errmsg = null;
            var user = _opsClient.GetUserByUsername(username);
            var passwordHash = user.Password.Trim();

            userId = user.Id;

            if (userId == 0)
                errmsg = "User does not exist";

            if (errmsg == null)
            {
                if (!VerifyHashPassword(oldPassword, passwordHash))
                    errmsg = "Invalid old password";
            }

            if (errmsg == null)
            {
                if (newPassword != retypeNewPassword)
                    errmsg = "New and retyped passwords do not match";
            }
            
            return errmsg;
        }

        public void ChangePassword(int userId, string password)
        {
            var passwordHash = GeneratePasswordHash(password);

            var user = new User {Password = passwordHash};
            _opsClient.PatchUser(userId, user);
        }

        private static bool VerifyHashPassword(string password, string hash)
        {
            var isValid = false;
            var tmpHash = GeneratePasswordHash(password);

            if (tmpHash == hash) isValid = true;

            return isValid;
        }

        public static string GeneratePasswordHash(string password)
        {
            var md5 = new MD5CryptoServiceProvider();

            byte[] tmpSource = Encoding.ASCII.GetBytes(password);
            byte[] tmpHash = md5.ComputeHash(tmpSource);

            var sOutput = new StringBuilder(tmpHash.Length);
            for (int i = 0; i < tmpHash.Length; i++)
            {
                sOutput.Append(tmpHash[i].ToString("X2")); // X2 formats to hexadecimal
            }
            return sOutput.ToString();
        }
    }
}
