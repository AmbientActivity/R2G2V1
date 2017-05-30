using Keebee.AAT.ApiClient.Clients;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class HomeController : Controller
    {
        // api client
        private readonly IUsersClient _usersClient;

        public HomeController()
        {
            _usersClient = new UsersClient();
        }

        public ActionResult Index()
        {
            return View();
        }

        // used by the KeepIISAlive Service
        [HttpGet]
        public void Ping()
        {
            _usersClient.GetCount();  // just get a count of users
        }
    }
}