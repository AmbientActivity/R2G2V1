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
            IEnumerable<string> msgs;
            var configid = c.Id;

            if (configid > 0)
            {
                msgs = Validate(c.Description);
                if (msgs == null)
                    UpdateConfig(c);
            }
            else
            {
                msgs = Validate(c.Description);
                if (msgs == null)
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
                .Select(config => new ConfigViewModel
                {
                    Id = config.Id,
                    Description = config.Description,
                    IsActive = config.IsActive,
                    CanEdit = config.Id != ConfigId.Default,
                    CanDelete = !_opsClient.GetActivityEventLogsForConfig(config.Id).Any() 
                        && !config.IsActive 
                        && ConfigId.Default != config.Id
                });

            return list;
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
                    SortOrder = cd.PhidgetType.Id,
                    PhidgetType = cd.PhidgetType.Description,
                    Description = cd.Description,
                    ResponseType = cd.ResponseType.Description,
                    CanDelete = !_opsClient.GetActivityEventLogsForConfigDetail(cd.Id).Any() && !config.IsActive
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
                Description = (config != null) ? config.Description : string.Empty
            };

            return vm;
        }

        private ConfigDetailEditViewModel LoadConfigDetailEditViewModel(int id, int configId)
        {
            ConfigDetail configDetail = null;
            IEnumerable<int> usedPhidgetIds;

            if (id > 0)
            {
                configDetail = _opsClient.GetConfigDetail(id);
                usedPhidgetIds = _opsClient.GetConfigWithDetails(configDetail.ConfigId)
                    .ConfigDetails
                    .Where(cd => cd.PhidgetType.Id != configDetail.PhidgetType.Id)
                    .Select(cd => cd.PhidgetType.Id);
            }
            else
            {
                usedPhidgetIds = _opsClient.GetConfigWithDetails(configId)
                    .ConfigDetails
                    .Select(cd => cd.PhidgetType.Id);
            }

            var allPhidgetTypes = _opsClient.GetPhidgetTypes().ToArray();
            var availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();

            var responseTypes = _opsClient.GetResponseTypes();

            var vm = new ConfigDetailEditViewModel
            {
                Id = configDetail?.Id ?? 0,
                Description = (configDetail != null) ? configDetail.Description : string.Empty,
                PhidgetTypes = new SelectList(availablePhidgetTypes, "Id", "Description", configDetail?.PhidgetType.Id),
                ResponseTypes = new SelectList(responseTypes, "Id", "Description", configDetail?.ResponseType.Id)
            };

            return vm;
        }

        private void UpdateConfig(ConfigEditViewModel config)
        {
            var c = new ConfigEdit
            {
                Description = config.Description
            };

            _opsClient.PatchConfig(config.Id, c);
        }

        private int AddConfig(ConfigEditViewModel config, int selectedConfigId)
        {
            var newConfig = new ConfigEdit
            {
                Description = config.Description
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
                ResponseTypeId = configDetail.ResponseTypeId
            };

            _opsClient.PatchConfigDetail(configDetail.Id, cd);
        }

        private int AddConfigDetail(ConfigDetailEditViewModel configDetail)
        {
            var cd = new ConfigDetailEdit
            {
                ConfigId = configDetail.ConfigId,
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