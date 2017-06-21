using Keebee.AAT.Shared;
using System;
using System.Security.Principal;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Keebee.AAT.Administrator
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["IsBeaconWatcherServiceInstalled"] = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher)
                ? "true" : "false";
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // get the authentication cookie
            var cookieName = FormsAuthentication.FormsCookieName;
            var authCookie = Context.Request.Cookies[cookieName];

            // if the cookie can't be found, don't issue the ticket
            if (authCookie == null) return;

            // get the authentication ticket and rebuild the principal & identity
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket == null) return;

            // all good
            var roles = authTicket.UserData.Split(new[] { '|' });
            var userIdentity = new GenericIdentity(authTicket.Name);
            var userPrincipal = new GenericPrincipal(userIdentity, roles);
            Context.User = userPrincipal;
        }
    }
}
