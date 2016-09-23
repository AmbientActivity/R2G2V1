using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ConfigsController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly CustomMessageQueue _messageQueueConfigSms;

        public ConfigsController()
        {
            _opsClient = new OperationsClient();

            _messageQueueConfigSms = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms
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
            var vm = new
            {
                ConfigList = GetConfigList(),
                ConfigDetailList = GetConfigDetailList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GetConfigEditView(int id, int selectedConfigId)
        {
            return PartialView("_ConfigEdit", LoadConfigEditViewModel(id, selectedConfigId));
        }

        [HttpGet]
        public PartialViewResult GetConfigDetailEditView(int id, int configId)
        {
            return PartialView("_ConfigDetailEdit", LoadConfigDetailEditViewModel(id, configId));
        }

        [HttpPost]
        public JsonResult Save(string config, int selectedConfigId)
        {
            var c = JsonConvert.DeserializeObject<ConfigEditViewModel>(config);
            var configid = c.Id;
            var rules = new ConfigRules {OperationsClient = _opsClient};

            var msgs = rules.Validate(c.Description, configid == 0);

            if (msgs == null)
            {
                if (configid > 0)
                    UpdateConfig(c);
                else
                    configid = AddConfig(c, selectedConfigId);
            }

            return Json(new
            {
                SelectedId = configid,
                ConfigList = GetConfigList(),
                ConfigDetailList = GetConfigDetailList(),
                Success = (null == msgs),
                ErrorMessages = msgs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _opsClient.DeleteConfig(id);

            return Json(new
            {
                ConfigList = GetConfigList(),
                Success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDetail(string configDetail)
        {
            var cd = JsonConvert.DeserializeObject<ConfigDetailEditViewModel>(configDetail);
            var configDetailid = cd.Id;
            var configurationRules = new ConfigRules();

            var msgs = configurationRules.ValidateDetail(cd.Description);

            if (msgs == null)
            {
                if (configDetailid > 0)
                    UpdateConfigDetail(cd);
                else
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
        public JsonResult DeleteDetail(int id, int configId, bool isActive)
        {
            _opsClient.DeleteConfigDetail(id);

            if (isActive)
                SendNotificationOfConfigChange(configId);

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
            SendNotificationOfConfigChange(configId);

            var vm = new
            {
                SelectedId = configId,
                ConfigList = GetConfigList(),
                ConfigDetailList = GetConfigDetailList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ConfigViewModel> GetConfigList()
        {
            var configs = _opsClient.GetConfigs().ToArray();

            var list = configs
                .Select(config => 
                {
                    {
                        var activityLogs = _opsClient.GetActivityEventLogsForConfig(config.Id);
                        var vm = new ConfigViewModel
                                {
                                    Id = config.Id,
                                    Description = config.Description,
                                    IsActive = config.IsActive,
                                    IsActiveEventLog = config.IsActiveEventLog,
                                    CanDelete = !activityLogs.Any()
                                                && !config.IsActive
                                                && ConfigId.Default != config.Id
                                };

                        return vm;
                    }
                });

            return list;
        }

        private IEnumerable<ConfigDetailViewModel> GetConfigDetailList()
        {
            var configs = _opsClient.GetConfigs();

            var list = configs
                .SelectMany(config => config.ConfigDetails
                    .Select(cd =>
                            {
                                var activityLogs = _opsClient.GetActivityEventLogsForConfigDetail(cd.Id);
                                var vm = new ConfigDetailViewModel
                                         {
                                             Id = cd.Id,
                                             ConfigId = config.Id,
                                             SortOrder = cd.PhidgetType.Id,
                                             PhidgetType = cd.PhidgetType.Description,
                                             PhidgetStyleType = cd.PhidgetStyleType.Description,
                                             Description = cd.Description,
                                             ResponseType = cd.ResponseType.Description,
                                             CanEdit = !activityLogs.Any()
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
                config = _opsClient.GetConfig(id);
            }
            else
            {
                selectedConfig = _opsClient.GetConfig(selectedConfigId);
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

        private ConfigDetailEditViewModel LoadConfigDetailEditViewModel(int id, int configId)
        {
            var configurationRules = new ConfigRules {OperationsClient = _opsClient};
            var configEdit = configurationRules.GetConfigEditViewModel(id, configId);
            var configDetail = configEdit.ConfigDetail;

            var vm = new ConfigDetailEditViewModel
            {
                Id = configDetail?.Id ?? 0,
                Description = (configDetail != null) ? configDetail.Description : string.Empty,
                PhidgetTypes = new SelectList(configEdit.PhidgetTypes, "Id", "Description", configDetail?.PhidgetType.Id),
                PhidgetStyleTypes = new SelectList(configEdit.PhidgetStyleTypes, "Id", "Description", configDetail?.PhidgetStyleType.Id),
                ResponseTypes = new SelectList(configEdit.ResponseTypes, "Id", "Description", configDetail?.ResponseType.Id)
            };

            return vm;
        }

        private void UpdateConfig(ConfigEditViewModel config)
        {
            var c = new ConfigEdit
            {
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog
            };

            _opsClient.PatchConfig(config.Id, c);

            if(config.IsActive)
                SendNotificationOfConfigChange(config.Id);
        }

        private int AddConfig(ConfigEditViewModel config, int selectedConfigId)
        {
            var newConfig = new ConfigEdit
            {
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog
            };

            var newId = _opsClient.PostConfig(newConfig);
            var selectedConfig = _opsClient.GetConfigWithDetails(selectedConfigId);

            foreach (var detail in selectedConfig.ConfigDetails)
            {
                _opsClient.PostConfigDetail(new ConfigDetailEdit
                        {
                            ConfigId = newId,
                            Description = detail.Description,
                            PhidgetTypeId = detail.PhidgetType.Id,
                            PhidgetStyleTypeId = detail.PhidgetStyleType.Id,
                            ResponseTypeId = detail.ResponseType.Id
                        });
            }
            
            return newId;
        }

        private void UpdateConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            var cd = new ConfigDetailEdit
            {
                Description = configDetail.Description,
                PhidgetTypeId = configDetail.PhidgetTypeId,
                PhidgetStyleTypeId = configDetail.PhidgetStyleTypeId,
                ResponseTypeId = configDetail.ResponseTypeId
            };

            _opsClient.PatchConfigDetail(configDetail.Id, cd);

            if (configDetail.IsActive)
                SendNotificationOfConfigChange(configDetail.ConfigId);
        }

        private int AddConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            var cd = new ConfigDetailEdit
            {
                ConfigId = configDetail.ConfigId,
                Description = configDetail.Description,
                PhidgetTypeId = configDetail.PhidgetTypeId,
                PhidgetStyleTypeId = configDetail.PhidgetStyleTypeId,
                ResponseTypeId = configDetail.ResponseTypeId
            };

            var id = _opsClient.PostConfigDetail(cd);

            if (configDetail.IsActive)
                SendNotificationOfConfigChange(configDetail.ConfigId);

            return id;
        }

        private void SendNotificationOfConfigChange(int configId)
        {
            var rules = new ConfigRules { OperationsClient = _opsClient };
            _messageQueueConfigSms.Send(rules.GetMessageBody(configId));
        }
    }
}