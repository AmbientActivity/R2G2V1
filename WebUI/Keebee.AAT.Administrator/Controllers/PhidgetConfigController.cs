using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PhidgetConfigController : Controller
    {
        // api client
        private readonly IConfigsClient _configsClient;

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

        [HttpGet]
        [Authorize]
        public PartialViewResult GetConfigDetailEditView(int id, int configId)
        {
            return PartialView("_ConfigDetailEdit", LoadConfigDetailEditViewModel(id, configId));
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
            var configs = new Config[0];
            var configid = -1;

            try
            {
                configid = config.Id;

                errMsg = configid > 0 
                    ? UpdateConfig(config) 
                    : AddConfig(config, selectedConfigId, out configid);

                configs = _configsClient.Get().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            var success = string.IsNullOrEmpty(errMsg);

            return Json(new
            {
                SelectedId = configid,
                ConfigList = GetConfigList(configs),
                ConfigDetailList = GetConfigDetailList(configs),
                Success = success,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id)
        {
            string errMsg;

            try
            {
                errMsg = _configsClient.Delete(id);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            var success = string.IsNullOrEmpty(errMsg);
            return Json(new
            {
                ConfigList = GetConfigList(_configsClient.Get().ToArray()),
                ErrorMessage = !success ? errMsg : null,
                Success = success,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult ValidateDetail(ConfigDetailEditViewModel configDetail)
        {
            IEnumerable<string> validateMsgs = null;
            string errMsg = null;

            try
            {
                var configurationRules = new PhidgetConfigRules();

                validateMsgs = configurationRules.ValidateDetail(configDetail.Description, configDetail.PhidgetTypeId, configDetail.PhidgetStyleTypeId);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ValidationMessages = validateMsgs,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveDetail(ConfigDetailEditViewModel configDetail)
        {
            string errMsg;
            var configDetailid = 0;
            var configs = new Config[0];

            try
            {
                configDetailid = configDetail.Id;

                errMsg = configDetailid > 0 
                    ? UpdateConfigDetail(configDetail) 
                    : AddConfigDetail(configDetail, out configDetailid);

                configs = _configsClient.Get().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            var success = string.IsNullOrEmpty(errMsg);
            return Json(new
            {
                ConfigDetailList = GetConfigDetailList(configs),
                SelectedId = configDetailid,
                Success = success,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteDetail(int id, int configId, bool isActive)
        {
            string errMsg;
            var configs = new Config[0];

            try
            {
                errMsg = _configsClient.DeleteDetail(id);

                if (isActive)
                    SendNewConfiguration(configId);

                configs = _configsClient.Get().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            var success = string.IsNullOrEmpty(errMsg);
            return Json(new
            {
                ConfigDetailList = GetConfigDetailList(configs),
                ErrorMessage = !success ? errMsg : null,
                Success = success,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult Activate(int configId)
        {
            string errMsg;
            Config[] configs = null;

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
                SelectedId = configId,
                ConfigList = GetConfigList(configs),
                ConfigDetailList = GetConfigDetailList(configs)
            }, JsonRequestBehavior.AllowGet);
        }

        private static IEnumerable<ConfigViewModel> GetConfigList(Config[] configs)
        {
            var defaultConfig = configs.Where(x => x.Id == ConfigId.Default).ToArray();
            var customConfigs = configs.Where(x => x.Id != ConfigId.Default).OrderBy(x => x.Description).ToArray();
            var allConfigs = defaultConfig.Union(customConfigs);

            var list = allConfigs
                .Select(config => 
                {
                    {
                        var vm = new ConfigViewModel
                                {
                                    Id = config.Id,
                                    Description = config.Description,
                                    IsActive = config.IsActive,
                                    IsActiveEventLog = config.IsActiveEventLog,
                                    CanDelete = !config.IsEventLogs
                                                && !config.IsActive
                                                && ConfigId.Default != config.Id
                                };

                        return vm;
                    }
                });

            return list;
        }

        private static IEnumerable<ConfigDetailViewModel> GetConfigDetailList(IEnumerable<Config> configs)
        {
            var list = configs
                .SelectMany(config => config.ConfigDetails
                    .Select(cd =>
                            {
                                var vm = new ConfigDetailViewModel
                                         {
                                             Id = cd.Id,
                                             ConfigId = config.Id,
                                             SortOrder = cd.PhidgetType.Id,
                                             PhidgetType = cd.PhidgetType.Description,
                                             PhidgetStyleType = cd.PhidgetStyleType.Description,
                                             Description = cd.Description,
                                             Location = cd.Location,
                                             ResponseType = cd.ResponseType.Description,
                                             InteractiveActivityTypeId = cd.ResponseType.InteractiveActivityType?.Id ?? 0,
                                             SwfFile = cd.ResponseType.InteractiveActivityType?.SwfFile ?? string.Empty,
                                             IsSystem = cd.ResponseType.IsSystem,
                                             CanEdit = !cd.IsEventLogs
                                };
                                return vm;
                            })).ToArray();
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

        private static ConfigDetailEditViewModel LoadConfigDetailEditViewModel(int id, int configId)
        {
            var configurationRules = new PhidgetConfigRules();
            var configEdit = configurationRules.GetConfigEditViewModel(id, configId);
            var configDetail = configEdit.ConfigDetail;

            var vm = new ConfigDetailEditViewModel
            {
                Id = configDetail?.Id ?? 0,
                Description = (configDetail != null) ? configDetail.Description : string.Empty,
                Location = (configDetail != null) ? configDetail.Location : string.Empty,
                PhidgetTypes = new SelectList(configEdit.PhidgetTypes, "Id", "Description", configDetail?.PhidgetType.Id),
                PhidgetStyleTypes = new SelectList(configEdit.PhidgetStyleTypes, "Id", "Description", configDetail?.PhidgetStyleType.Id),
                ResponseTypes = new SelectList(configEdit
                    .ResponseTypes
                    .Select(x => new
                        {
                            x.Id,
                            Description = $"({x.ResponseTypeCategory.Description}) - {x.Description}"
                        })
                    .OrderBy(x => x.Description), 
                    "Id", "Description", configDetail?.ResponseType.Id)
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

        private string UpdateConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            string errMsg;
            try
            {
                var cd = new ConfigDetailEdit
                {
                    Description = configDetail.Description,
                    Location = configDetail.Location,
                    PhidgetTypeId = configDetail.PhidgetTypeId,
                    PhidgetStyleTypeId = configDetail.PhidgetStyleTypeId,
                    ResponseTypeId = configDetail.ResponseTypeId
                };

                errMsg = _configsClient.PatchDetail(configDetail.Id, cd);
                if (errMsg != null) return errMsg;

                if (configDetail.IsActive)
                    SendNewConfiguration(configDetail.ConfigId);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        private string AddConfigDetail(ConfigDetailEditViewModel configDetail, out int newId)
        {
            newId = -1;
            string errMsg;

            try
            {
                var cd = new ConfigDetailEdit
                {
                    ConfigId = configDetail.ConfigId,
                    Description = configDetail.Description,
                    Location = configDetail.Location,
                    PhidgetTypeId = configDetail.PhidgetTypeId,
                    PhidgetStyleTypeId = configDetail.PhidgetStyleTypeId,
                    ResponseTypeId = configDetail.ResponseTypeId
                };

                errMsg = _configsClient.PostDetail(cd, out newId);

                if (configDetail.IsActive)
                    SendNewConfiguration(configDetail.ConfigId);
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
    }
}