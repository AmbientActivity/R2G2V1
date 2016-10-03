﻿using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.RESTClient;
using Keebee.AAT.BusinessRules.DTO;
using Keebee.AAT.SystemEventLogging;
using System.Web.Mvc;
using System.Web.Security;
using Keebee.AAT.Administrator.ViewModels;

namespace Keebee.AAT.Administrator.Controllers
{
    public class HomeController : Controller
    {
        private readonly OperationsClient _opsClient;

        public HomeController()
        {
            _opsClient = new OperationsClient();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(LoginViewModel vm)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var rules = new UserRules { OperationsClient = _opsClient };
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
            var rules = new UserRules {OperationsClient = _opsClient};
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