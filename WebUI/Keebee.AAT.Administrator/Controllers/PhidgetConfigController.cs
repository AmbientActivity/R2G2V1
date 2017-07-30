using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PhidgetConfigController : Controller
    {
        // api client
        private readonly IConfigsClient _configsClient;

        // SMS message queue sender
        private readonly CustomMessageQueue _messageQueueConfigSms;

        public PhidgetConfigController()
        {
            _configsClient = new ConfigsClient();

            _messageQueueConfigSms = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms
            });
        }

        // GET: PhidgetConfig
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            string errMsg = null;
            var configs = new Config[0];

            try
            {
                configs = _configsClient.Get().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                ConfigList = GetConfigList(configs),
                ConfigDetailList = GetConfigDetailList(configs)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetConfigEditView(int id, int selectedConfigId)
        {
            return PartialView("_ConfigEdit", LoadConfigEditViewModel(id, selectedConfigId));
        }

        [HttpPost]
        [Authorize]
        public JsonResult Validate(ConfigEditViewModel config, int selectedConfigId)
        {
            IEnumerable<string> validateMsgs = null;
            string errMsg = null;

            try
            {
                var configid = config.Id;
                var rules = new PhidgetConfigRules();

                validateMsgs = rules.Validate(config.Description, configid == 0);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ValidationMessages = validateMsgs,
                ErrorMessage = errMsg,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Save(ConfigEditViewModel config, int selectedConfigId)
        {
            string errMsg;
            ConfigViewModel vm = null;
            ConfigDetailViewModel[] detailVm = null;

            try
            {
                var configid = config.Id;

                if (configid > 0)
                {
                    errMsg = UpdateConfig(config);
                    var c = _configsClient.Get(configid);
                    vm = GetConfigViewModel(c);
                }
                else
                {
                    errMsg = AddConfig(config, selectedConfigId, out configid);
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception();

                    var cfg = _configsClient.GetWithDetails(configid);
                    vm = GetConfigViewModel(cfg);
                    detailVm = cfg.ConfigDetails.Select(GetConfigDetailViewModel)
                        .ToArray();
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                ConfigList = vm != null
                    ? new Collection<ConfigViewModel> { vm }
                    : new Collection<ConfigViewModel> { new ConfigViewModel() },
                ConfigDetailList = detailVm
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id)
        {
            string errMsg;
            var deletedId = 0;

            try
            {
                errMsg = _configsClient.Delete(id);
                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                deletedId = id;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                DeletedId = deletedId
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult Activate(int configId)
        {
            string errMsg;
            var configs = new Config[0];

            try
            {
                errMsg = _configsClient.Activate(configId);

                if (!string.IsNullOrEmpty(errMsg))
                    throw new Exception(errMsg);

                SendNewConfiguration(configId);

                configs = _configsClient.Get().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                ConfigList = GetConfigList(configs)
            }, JsonRequestBehavior.AllowGet);
        }

        private static IEnumerable<ConfigViewModel> GetConfigList(Config[] configs)
        {
            var defaultConfig = configs.Where(x => x.Id == ConfigId.Default).ToArray();
            var customConfigs = configs.Where(x => x.Id != ConfigId.Default).OrderBy(x => x.Description).ToArray();
            var allConfigs = defaultConfig.Union(customConfigs);

            var list = allConfigs.Select(GetConfigViewModel);

            return list;
        }

        private static IEnumerable<ConfigDetailViewModel> GetConfigDetailList(IEnumerable<Config> configs)
        {
            var list = configs
                .SelectMany(config => config.ConfigDetails
                    .Select(GetConfigDetailViewModel))
                        .ToArray();

            return list;
        }

        private ConfigEditViewModel LoadConfigEditViewModel(int id, int selectedConfigId)
        {
            Config config = null;
            Config selectedConfig = null;

            if (id > 0)
            {
                config = _configsClient.Get(id);
            }
            else
            {
                selectedConfig = _configsClient.Get(selectedConfigId);
            }

            var vm = new ConfigEditViewModel
            {
                Id = config?.Id ?? 0,
                SourceConfigName = selectedConfig?.Description,
                Description = (config != null) ? config.Description : string.Empty,
                IsActiveEventLog = config?.IsActiveEventLog ?? false
            };

            return vm;
        }

        private string UpdateConfig(ConfigViewModel config)
        {
            string errMsg;

            try
            {
                var c = new ConfigEdit
                {
                    Description = config.Description,
                    IsActiveEventLog = config.IsActiveEventLog
                };

                errMsg = _configsClient.Patch(config.Id, c);

                if (config.IsActive)
                    SendNewConfiguration(config.Id);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        private string AddConfig(ConfigViewModel config, int selectedConfigId, out int newId)
        {
            string errMsg;
            newId = -1;

            try
            {
                // duplicate config
                var newConfig = new ConfigEdit
                {
                    Description = config.Description,
                    IsActiveEventLog = config.IsActiveEventLog
                };

                errMsg = _configsClient.Post(newConfig, out newId);
                if (!string.IsNullOrEmpty(errMsg))
                    throw new Exception(errMsg);

                // duplicate config details
                var rules = new PhidgetConfigRules();
                errMsg = rules.DuplicateConfigDetails(selectedConfigId, newId);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        private void SendNewConfiguration(int configId)
        {
            var rules = new PhidgetConfigRules();
            _messageQueueConfigSms.Send(rules.GetMessageBody(configId));
        }

        private static ConfigViewModel GetConfigViewModel(Config config)
        {
            return new ConfigViewModel
            {
                Id = config.Id,
                Description = config.Description,
                IsActive = config.IsActive,
                IsActiveEventLog = config.IsActiveEventLog,
                CanDelete = !config.IsEventLogs
                                && !config.IsActive
                                && ConfigId.Default != config.Id
            };
        }

        private static ConfigDetailViewModel GetConfigDetailViewModel(ConfigDetail detail)
        {
            return new ConfigDetailViewModel
            {
                Id = detail.Id,
                ConfigId = detail.ConfigId,
                SortOrder = detail.PhidgetType.Id,
                PhidgetType = detail.PhidgetType.Description,
                PhidgetStyleType = detail.PhidgetStyleType.Description,
                Description = detail.Description,
                Location = detail.Location ?? string.Empty,
                ResponseType = detail.ResponseType.Description,
                IsSystem = detail.ResponseType.ResponseTypeCategory.Id == ResponseTypeCategoryId.System,
                CanEdit = !detail.IsEventLogs
            };
        }
    }
}