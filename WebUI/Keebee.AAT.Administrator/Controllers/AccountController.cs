using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Keebee.AAT.Administrator.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(LoginViewModel vm)
        {
            string errMsg;

            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                var rules = new AccountRules();
                int userId;

                errMsg = rules.Login(vm.Username, vm.Password, out userId);

                if (!string.IsNullOrEmpty(errMsg))
                    throw new Exception(errMsg);

                if (userId <= 0)
                    throw new Exception("User not found");

                CreateLoginAuthenticationTicket(vm.Username, userId);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateUser(LoginViewModel vm)
        {
            string errMsg;
            string validateMsg = null;

            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                var rules = new AccountRules();
                validateMsg = rules.ValidateLogin(vm.Username, vm.Password, out errMsg);

                if (!string.IsNullOrEmpty(errMsg))
                    throw new Exception(errMsg);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                ValidationMessage = validateMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public PartialViewResult GetChangePasswordView()
        {
            return PartialView("_ChangePassword", new ChangePasswordViewModel());
        }

        [HttpGet]
        public PartialViewResult GetResetCaregiverView()
        {
            return PartialView("_ResetCaregiver");
        }

        [HttpGet]
        [Authorize]
        public JsonResult ResetCaregiver()
        {
            string errMsg;

            try
            {
                var rules = new AccountRules();
                errMsg = rules.ResetCaregiver();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = true,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult ChangePassword(ChangePasswordViewModel vm)
        {
            string errMsg;

            try
            {
                var username = System.Web.HttpContext.Current.User.Identity.Name;
                var rules = new AccountRules();

                User user;
                errMsg = rules.GetByUsername(username, out user);

                if (!string.IsNullOrEmpty(errMsg))
                    throw new Exception(errMsg);

                if (user.Id <= 0)
                    throw new Exception("User not found");

                errMsg = rules.ChangePassword(user.Id, vm.NewPassword);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = true,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult ValidatePasswordChange(ChangePasswordViewModel vm)
        {
            string errMsg = null;
            string validateMsg = null;

            try
            {
                var username = System.Web.HttpContext.Current.User.Identity.Name;
                int userId;

                var rules = new AccountRules();
                validateMsg = rules.ValidatePasswordChange(username, vm.OldPassword, vm.NewPassword, vm.RetypedNewPassword, out userId);

                if (userId <= 0)
                    throw new Exception("User not found");
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = true,
                ErrorMessage = errMsg,
                ValidationMessage = validateMsg
            }, JsonRequestBehavior.AllowGet);
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