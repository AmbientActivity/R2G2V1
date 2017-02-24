using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Keebee.AAT.Administrator.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(LoginViewModel vm)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var rules = new AccountRules();
            string errmsg;

            var userId = rules.AttemptToLogin(vm.Username, vm.Password, out errmsg);
            var success = (userId > 0);

            if (success)
            {
                CreateLoginAuthenticationTicket(vm.Username, userId);
            }

            return Json(new
            {
                Success = success,
                ErrorMessage = errmsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public PartialViewResult GetLoginView()
        {
            return PartialView("_Login", LoadLoginViewModel());
        }

        [HttpGet]
        public PartialViewResult GetChangePasswordView()
        {
            return PartialView("_ChangePassword", new ChangePasswordViewModel());
        }

        [HttpGet]
        [Authorize]
        public JsonResult AttemptToChangePassword(ChangePasswordViewModel vm)
        {
            var username = System.Web.HttpContext.Current.User.Identity.Name;
            int userId;

            var rules = new AccountRules();
            var errmsg = rules.ValidatePasswordChange(username, vm.OldPassword, vm.NewPassword, vm.RetypedNewPassword, out userId);
            var success = (userId > 0);

            if (success)
            {
                rules.ChangePassword(userId, vm.NewPassword);
            }

            return Json(new
            {
                Success = success,
                ErrorMessage = errmsg
            }, JsonRequestBehavior.AllowGet);
        }

        private static LoginViewModel LoadLoginViewModel()
        {
            var vm = new LoginViewModel
            {
                Usernames = new SelectList(new Collection<SelectListItem>
                    {
                        new SelectListItem {Value = "admin", Text = "Administrator"},
                        new SelectListItem {Value = "caregiver", Text = "Caregiver"}
                    },
                    "Value", "Text", "caregiver")
            };

            return vm;
        }

        [HttpGet]
        public string GetCurrentSessionUser()
        {
            return (System.Web.HttpContext.Current.User.Identity.Name);
        }

        private void CreateLoginAuthenticationTicket(string username, int userId)
        {
            var rules = new AccountRules();
            var roles = rules.GetUserRoles(userId);

            var cookieTimeoutMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["CookieTimeoutMinutes"]);
            var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now,
                DateTime.Now.AddMinutes(cookieTimeoutMinutes), false, roles);

            var enticket = FormsAuthentication.Encrypt(ticket);
            var cname = FormsAuthentication.FormsCookieName;

            Response.Cookies.Add(new HttpCookie(cname, enticket));
        }
    }
}