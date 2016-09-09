using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ConfigsController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly CustomMessageQueue _messageQueueConfig;

        public ConfigsController()
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
            return View(LoadConfigViewModel());
        }

        private ConfigViewModel LoadConfigViewModel()
        {
            var configs = _opsClient.GetConfigs();

            var configList = configs
                .Select(c => new
                             {
                                 c.Id,
                                 c.Description
                             }).ToArray();

            var vm = new ConfigViewModel
                {
                    Configs = new SelectList(configList, "Id", "Description")
                };

            return vm;
        }

        // POST: Activate?configId=2
        public void Activate(int configId)
        {
            _opsClient.PostActivateConfig(configId);
            _messageQueueConfig.Send("1");
        }
    }
}