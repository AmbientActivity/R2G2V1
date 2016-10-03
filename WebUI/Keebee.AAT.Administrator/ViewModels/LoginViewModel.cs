using System.Web.Mvc;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public SelectList Usernames { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}