using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PhidgetConfigDetailsController : Controller
    {
        // api client
        private readonly IConfigsClient _configsClient;

        // SMS message queue sender
        private readonly CustomMessageQueue _messageQueueConfigSms;

        public PhidgetConfigDetailsController()
        {
            _configsClient = new ConfigsClient();

            _messageQueueConfigSms = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms
            });
        }

        [HttpPost]
        [Authorize]
        public JsonResult Validate(ConfigDetailEditViewModel configDetail)
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
        public JsonResult Save(ConfigDetailEditViewModel configDetail)
        {
            string errMsg;
            ConfigDetailViewModel vm = null;

            try
            {
                var configDetailid = configDetail.Id;

                errMsg = configDetailid > 0
                    ? UpdateConfigDetail(configDetail)
                    : AddConfigDetail(configDetail, out configDetailid);

                if (configDetailid > 0)
                {
                    var detail = _configsClient.GetDetail(configDetailid);
                    vm = GetConfigDetailModel(detail);
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
                ConfigDetailList = vm != null
                    ? new Collection<ConfigDetailViewModel> { vm }
                    : new Collection<ConfigDetailViewModel> { new ConfigDetailViewModel() }

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id, int configId, bool isActive)
        {
            string errMsg;
            var deletedId = 0;

            try
            {
                errMsg = _configsClient.DeleteDetail(id);

                if (isActive)
                    SendNewConfiguration(configId);

                deletedId = id;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            var success = string.IsNullOrEmpty(errMsg);
            return Json(new
            {
                DeletedId = deletedId,
                ErrorMessage = !success ? errMsg : null,
                Success = success,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetConfigDetailEditView(int id, int configId)
        {
            return PartialView("_ConfigDetailEdit", LoadConfigDetailEditViewModel(id, configId));
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
                        Description = $"{x.ResponseTypeCategory.Description} ({x.Description})"
                    })
                    .OrderBy(x => x.Description),
                    "Id", "Description", configDetail?.ResponseType.Id),
                PhidgetStyleTypeIdOnOff = PhidgetStyleTypeId.OnOff,
                PhidgetStyleTypeIdTouch = PhidgetStyleTypeId.Touch,
                MaxPhidgetTypeIdSensor = PhidgetTypeId.Sensor7
            };

            return vm;
        }

        private void SendNewConfiguration(int configId)
        {
            var rules = new PhidgetConfigRules();
            _messageQueueConfigSms.Send(rules.GetMessageBody(configId));
        }

        private static ConfigDetailViewModel GetConfigDetailModel(ConfigDetail detail)
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