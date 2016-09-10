using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Keebee.AAT.BusinessRules;
using Newtonsoft.Json;

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
                    Description = c.Description,
                    CanDelete = !_opsClient.GetActivityEventLogsForConfig(c.Id).Any()
                }),
                ConfigDetailList = GetConfigDetailList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GetConfigDetailEditView(int id)
        {
            return PartialView("_ConfigDetailEdit", LoadConfigDetailEditViewModel(id));
        }

        [HttpPost]
        public JsonResult SaveDetail(string configDetail)
        {
            var cd = JsonConvert.DeserializeObject<ConfigDetailEditViewModel>(configDetail);
            IEnumerable<string> msgs;
            var configDetailid = cd.Id;

            if (configDetailid > 0)
            {
                msgs = ValidateDetail(cd.Description);
                if (msgs == null)
                    UpdateConfigDetail(cd);
            }
            else
            {
                msgs = ValidateDetail(cd.Description);
                if (msgs == null)
                    configDetailid = AddConfigDetail(cd);
            }

            return Json(new
            {
                ConfigDetailList = GetConfigDetailList(),
                SelectedId = configDetailid,
                Success = (null == msgs),
                ErrorMessages = msgs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDetail(int id)
        {
            _opsClient.DeleteConfigDetail(id);

            return Json(new
            {
                ConfigDetailList = GetConfigDetailList(),
                Success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: Activate?configId=2
        public JsonResult Activate(int configId)
        {
            _opsClient.PostActivateConfig(configId);
            _messageQueueConfig.Send("1");

            var config = _opsClient.GetConfig(configId);

            var vm = new
            {
                ActiveConfig = new ConfigViewModel
                {
                    Id = config.Id,
                    Description = config.Description
                }
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
                    Id = cd.Id,
                    ConfigId = config.Id,
                    PhidgetType = cd.PhidgetType.Description,
                    Description = cd.Description,
                    ResponseType = cd.ResponseType.Description,
                    CanDelete = !_opsClient.GetActivityEventLogsForConfigDetail(cd.Id).Any()
                }));

            return list;
        }

        private ConfigDetailEditViewModel LoadConfigDetailEditViewModel(int id)
        {
            ConfigDetail configDetail = null;
            //IEnumerable<ConfigDetail> configDetails = null;

            if (id > 0)
            {
                //configDetails = _opsClient.Get();
                configDetail = _opsClient.GetConfigDetail(id);
            }

            var phidgetTypes = _opsClient.GetPhidgetTypes();
            var responseTypes = _opsClient.GetResponseTypes();

            var vm = new ConfigDetailEditViewModel
            {
                Id = configDetail?.Id ?? 0,
                Description = (configDetail != null) ? configDetail.Description : string.Empty,
                PhidgetTypes = new SelectList(phidgetTypes, "Id", "Description", configDetail?.PhidgetType.Id),
                ResponseTypes = new SelectList(responseTypes, "Id", "Description", configDetail?.ResponseType.Id)
            };

            return vm;
        }

        private void UpdateConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            var cd = new ConfigDetailEdit
            {
                Description = configDetail.Description,
                PhidgetTypeId = configDetail.PhidgetTypeId,
                ResponseTypeId = configDetail.ResponseTypeId
            };

            _opsClient.PatchConfigDetail(configDetail.Id, cd);
        }

        private int AddConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            var cd = new ConfigDetailEdit
            {
                Description = configDetail.Description,
                PhidgetTypeId = configDetail.PhidgetTypeId,
                ResponseTypeId = configDetail.ResponseTypeId
            };

            var id = _opsClient.PostConfigDetail(cd);

            return id;
        }

        private static IEnumerable<string> Validate(string description)
        {
            return ValidationRules.ValidateConfig(description);
        }

        private static IEnumerable<string> ValidateDetail(string description)
        {
            return ValidationRules.ValidateConfigDetail(description);
        }
    }
}