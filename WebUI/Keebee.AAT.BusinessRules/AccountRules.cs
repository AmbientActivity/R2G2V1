using System;
using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Keebee.AAT.Shared;

namespace Keebee.AAT.BusinessRules
{
    public class AccountRules
    {
        private readonly IUsersClient _usersClient;

        public AccountRules()
        {
            _usersClient = new UsersClient();
        }

        public int CreateUser(UserModel user)
        {
            var u = new User
            {
                Id = user.Id,
                Username = user.Username,
                Password = GeneratePasswordHash(user.Password)
            };

            var id = _usersClient.Post(u);

            return id;
        }

        public string GetByUsername(string username, out User user)
        {
            string errMsg = null;
            user = null;

            try
            {
                user = _usersClient.GetByUsername(username);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;

            }
            return errMsg;
        }

        public string Login(string username, string password, out int userId)
        {
            string errMsg = null;
            userId = -1;

            try
            {
                var user = _usersClient.GetByUsername(username);

                if (user != null)
                    userId = user.Id;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        public string ValidateLogin(string username, string password, out string errMsg)
        {
            errMsg = null;
            string validateMsg = null;

            try
            {
                var user = _usersClient.GetByUsername(username);

                if (user.Id == 0)
                    validateMsg = "User does not exist";

                if (string.IsNullOrEmpty(validateMsg))
                {
                    if (string.IsNullOrEmpty(password))
                        validateMsg = "Password is required";
                }

                if (string.IsNullOrEmpty(validateMsg))
                {
                    if (!VerifyHashPassword(password, user.Password.Trim()))
                        validateMsg = "Incorrect passowrd";
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return validateMsg;
        }

        public string GetUserRoles(int userId)
        {
            var userRolesClient = new UserRolesClient();
            var user = userRolesClient.GetByUser(userId).Single();
            var s = new StringBuilder();

            foreach (var r in user.Roles)
            {
                s.Append(r.Description + "|");
            }

            return s.ToString().Substring(0, s.ToString().Length - 1);
        }

        public string ValidatePasswordChange(string username, string existingPassword, string newPassword, string retypeNewPassword, out int userId)
        {
            string validateMsg = null;
            userId = -1;

            if (string.IsNullOrEmpty(existingPassword))
                return "Old password cannot be blank";

            if (string.IsNullOrEmpty(newPassword) && string.IsNullOrEmpty(retypeNewPassword))
                return "New password cannot be blank";

            var user = _usersClient.GetByUsername(username);
            var passwordHash = user.Password.Trim();

            userId = user.Id;

            if (userId == 0)
                validateMsg = "User does not exist";

            if (string.IsNullOrEmpty(validateMsg))
            {
                if (!VerifyHashPassword(existingPassword, passwordHash))
                    validateMsg = "Invalid old password";
            }

            if (string.IsNullOrEmpty(validateMsg))
            {
                if (newPassword != retypeNewPassword)
                    validateMsg = "New and retyped passwords do not match";
            }
            
            return validateMsg;
        }

        public string ChangePassword(int userId, string password)
        {
            var passwordHash = GeneratePasswordHash(password);
            var user = new User {Password = passwordHash};

            return _usersClient.Patch(userId, user);
        }

        public string ResetCaregiver()
        {
            var caregiver = _usersClient.Get(UserId.Caregiver);
            caregiver.Password = GeneratePasswordHash(UserDefaultPassword.Caregiver);

            return _usersClient.Patch(UserId.Caregiver, caregiver);
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

            var tmpSource = Encoding.ASCII.GetBytes(password);
            var tmpHash = md5.ComputeHash(tmpSource);

            var sOutput = new StringBuilder(tmpHash.Length);
            foreach (var b in tmpHash)
            {
                sOutput.Append(b.ToString("X2")); // X2 formats to hexadecimal
            }
            return sOutput.ToString();
        }
    }
}
