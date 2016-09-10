using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ConfigurationsController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly CustomMessageQueue _messageQueueConfig;

        public ConfigurationsController()
        {
            _opsClient = new OperationsClient();

            _messageQueueConfig = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Config
            });
        }

        // GET: Configs
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData()
        {
            var configs = _opsClient.GetConfigs().ToArray();

            var vm = new
            {
                ActiveConfig = configs.Where(c => c.IsActive).Select(c => new ConfigViewModel
                {
                    Id = c.Id,
                    Description = c.Description
                }).Single(),
                ConfigList = configs.Select(c => new ConfigViewModel
                {
                    Id = c.Id,
                    Description = c.Description
                }),
                ConfigDetailList = GetConfigDetailList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ConfigDetailViewModel> GetConfigDetailList()
        {
            var configs = _opsClient.GetConfigs();

            var list = configs
                .SelectMany(config => config.ConfigDetails
                .Select(cd => new ConfigDetailViewModel
                {
                    Id = config.Id,
                    ActivityType = cd.ActivityType.Description,
                    ResponseType = cd.ResponseType.Description,
                    Phidget = cd.ActivityType.PhidgetType,
                    IsSystem = cd.ResponseType.IsSystem
                }));

            return list;
        }

        // POST: Activate?configId=2
        public JsonResult Activate(int configId)
        {
            _opsClient.PostActivateConfig(configId);
            _messageQueueConfig.Send("1");

            var config = _opsClient.GetConfig(configId);

            var vm = new
            {
                ActiveConfig =  new ConfigViewModel
                {
                    Id = config.Id,
                    Description = config.Description
                }
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }
    }
}