﻿using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PhidgetConfigController : Controller
    {
        // api client
        private readonly IConfigsClient _configsClient;
        private readonly IActivityEventLogsClient _activityEventLogsClient;

        private readonly CustomMessageQueue _messageQueueConfigSms;

        public PhidgetConfigController()
        {
            _configsClient = new ConfigsClient();
            _activityEventLogsClient = new ActivityEventLogsClient();

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
            var vm = new
            {
                ConfigList = GetConfigList(),
                ConfigDetailList = GetConfigDetailList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
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
        public JsonResult Save(string config, int selectedConfigId)
        {
            var c = JsonConvert.DeserializeObject<ConfigEditViewModel>(config);
            var configid = c.Id;
            var rules = new PhidgetConfigRules();

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
        [Authorize]
        public JsonResult Delete(int id)
        {
            _configsClient.Delete(id);

            return Json(new
            {
                ConfigList = GetConfigList(),
                Success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveDetail(string configDetail)
        {
            var cd = JsonConvert.DeserializeObject<ConfigDetailEditViewModel>(configDetail);
            var configDetailid = cd.Id;
            var configurationRules = new PhidgetConfigRules();

            var msgs = configurationRules.ValidateDetail(cd.Description, cd.PhidgetTypeId, cd.PhidgetStyleTypeId);

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
        [Authorize]
        public JsonResult DeleteDetail(int id, int configId, bool isActive)
        {
            _configsClient.DeleteDetail(id);

            if (isActive)
                SendNewConfiguration(configId);

            return Json(new
            {
                ConfigDetailList = GetConfigDetailList(),
                Success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: Activate?configId=2
        [Authorize]
        public JsonResult Activate(int configId)
        {
            _configsClient.Activate(configId);
            SendNewConfiguration(configId);

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
            var configs = _configsClient.Get().ToArray();

            var defaultConfig = configs.Where(x => x.Id == ConfigId.Default).ToArray();
            var customConfigs = configs.Where(x => x.Id != ConfigId.Default).OrderBy(x => x.Description).ToArray();
            var allConfigs = defaultConfig.Union(customConfigs);

            var list = allConfigs
                .Select(config => 
                {
                    {
                        var activityLogs = _activityEventLogsClient.GetForConfig(config.Id);
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
            var configs = _configsClient.Get();

            var list = configs
                .SelectMany(config => config.ConfigDetails
                    .Select(cd =>
                            {
                                var activityLogs = _activityEventLogsClient.GetForConfigDetail(cd.Id);
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
                                             IsSystem = cd.ResponseType.IsSystem,
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

        private ConfigDetailEditViewModel LoadConfigDetailEditViewModel(int id, int configId)
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

            _configsClient.Patch(config.Id, c);

            if(config.IsActive)
                SendNewConfiguration(config.Id);
        }

        private int AddConfig(ConfigEditViewModel config, int selectedConfigId)
        {
            // config
            var newConfig = new ConfigEdit
            {
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog
            };

            var newId = _configsClient.Post(newConfig);

            // config details
            var rules = new PhidgetConfigRules();
            rules.DuplicateConfigDetails(selectedConfigId, newId);

            return newId;
        }

        private void UpdateConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            var cd = new ConfigDetailEdit
            {
                Description = configDetail.Description,
                Location = configDetail.Location,
                PhidgetTypeId = configDetail.PhidgetTypeId,
                PhidgetStyleTypeId = configDetail.PhidgetStyleTypeId,
                ResponseTypeId = configDetail.ResponseTypeId
            };

            _configsClient.PatchDetail(configDetail.Id, cd);

            if (configDetail.IsActive)
                SendNewConfiguration(configDetail.ConfigId);
        }

        private int AddConfigDetail(ConfigDetailEditViewModel configDetail)
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

            var id = _configsClient.PostDetail(cd);

            if (configDetail.IsActive)
                SendNewConfiguration(configDetail.ConfigId);

            return id;
        }

        private void SendNewConfiguration(int configId)
        {
            var rules = new PhidgetConfigRules();
            _messageQueueConfigSms.Send(rules.GetMessageBody(configId));
        }
    }
}