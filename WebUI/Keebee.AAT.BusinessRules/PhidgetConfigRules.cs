using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace Keebee.AAT.BusinessRules
{
    public class PhidgetConfigRules
    {
        private readonly IConfigsClient _configsClient;
        private readonly IResponseTypesClient _responseTypesClient;
        private readonly IPhidgetTypesClient _phidgetTypesClient;
        private readonly IPhidgetStyleTypesClient _phidgetStyleTypesClient;

        public PhidgetConfigRules()
        {
            _configsClient = new ConfigsClient();
            _responseTypesClient = new ResponseTypesClient();
            _phidgetTypesClient = new PhidgetTypesClient();
            _phidgetStyleTypesClient = new PhidgetStyleTypesClient();
        }

        // validation
        public List<string> Validate(string description, bool addnew)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Configuration name is required");

            if (!addnew) return msgs.Count > 0 ? msgs : null;
            if (description?.Length == 0) return msgs;

            var config = _configsClient.GetByDescription(description);

            if (config.Description != null)
                msgs.Add($"A configuration with the name '{description}' already exists");

            return msgs.Count > 0 ? msgs : null;
        }

        public List<string> ValidateDetail(string description, int phidgetTypeId, int phidgetStyleTypeId)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Description is required");

            var validPhidgetMsg = ValidatePhidgetStyle(phidgetTypeId, phidgetStyleTypeId);

            if (validPhidgetMsg != null)
                msgs.AddRange(validPhidgetMsg);

            return msgs.Count > 0 ? msgs : null;
        }

        public List<string> ValidatePhidgetStyle(int phidgetTypeId, int phidgetStyleTypeId)
        {
            var msgs = new List<string>();

            switch (phidgetTypeId)
            {
                case PhidgetTypeId.Sensor0:
                case PhidgetTypeId.Sensor1:
                case PhidgetTypeId.Sensor2:
                case PhidgetTypeId.Sensor3:
                case PhidgetTypeId.Sensor4:
                case PhidgetTypeId.Sensor5:
                case PhidgetTypeId.Sensor6:
                case PhidgetTypeId.Sensor7:
                    if (phidgetStyleTypeId == PhidgetStyleTypeId.OnOff || phidgetStyleTypeId == PhidgetStyleTypeId.OnOnly || phidgetStyleTypeId == PhidgetStyleTypeId.NonRotational)
                        msgs.Add("Sensors cannot be of type On/Off, On Only or Non-rotational");
                    break;
                case PhidgetTypeId.Input0:
                case PhidgetTypeId.Input1:
                case PhidgetTypeId.Input2:
                case PhidgetTypeId.Input3:
                case PhidgetTypeId.Input4:
                case PhidgetTypeId.Input5:
                case PhidgetTypeId.Input6:
                case PhidgetTypeId.Input7:
                    if (phidgetStyleTypeId != PhidgetStyleTypeId.OnOff && phidgetStyleTypeId != PhidgetStyleTypeId.OnOnly && phidgetStyleTypeId != PhidgetStyleTypeId.NonRotational)
                        msgs.Add("Inputs must be of type On/Off, On Only or Non-rotational");
                    break;
            }

            return msgs.Count > 0 ? msgs : null;
        }

        // duplicate
        public string DuplicateConfigDetails(int selectedConfigId, int newConfigId)
        {
            string errMsg = null;

            try
            {
                var selectedConfig = _configsClient.GetWithDetails(selectedConfigId);

                foreach (var detail in selectedConfig.ConfigDetails)
                {
                    int newId;
                    errMsg = _configsClient.PostDetail(new ConfigDetailEdit
                    {
                        ConfigId = newConfigId,
                        Description = detail.Description,
                        Location = detail.Location,
                        PhidgetTypeId = detail.PhidgetType.Id,
                        PhidgetStyleTypeId = detail.PhidgetStyleType.Id,
                        ResponseTypeId = detail.ResponseType.Id
                    }, out newId);

                    if (!string.IsNullOrEmpty(errMsg))
                        throw new Exception(errMsg);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        // view model
        public ConfigDetailModel GetConfigEditViewModel(int id, int configId)
        {
            var availableResponseTypes = _responseTypesClient.Get();
            var allPhidgetTypes = _phidgetTypesClient.Get().ToArray();
            var availablePhidgetStyleTypes = _phidgetStyleTypesClient.Get().ToArray();

            ConfigDetail configDetail = null;

            IEnumerable<int> usedPhidgetIds;
            IEnumerable<PhidgetType> availablePhidgetTypes;
            // edit mode
            if (id > 0)
            {
                configDetail = _configsClient.GetDetail(id);

                // get all unused phidgets plus the one being edited
                usedPhidgetIds = _configsClient.GetWithDetails(configDetail.ConfigId)
                    .ConfigDetails
                    .Where(cd => cd.PhidgetType.Id != configDetail.PhidgetType.Id)
                    .Select(cd => cd.PhidgetType.Id);

                availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();
            }
            else // add mode
            {
                // get only unused phidgets
                usedPhidgetIds = _configsClient.GetWithDetails(configId)
                    .ConfigDetails
                    .Select(cd => cd.PhidgetType.Id);

                availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();
            }

            return new ConfigDetailModel
            {
                Id = configDetail?.Id ?? 0,
                ConfigDetail = configDetail,
                Description = (configDetail != null) ? configDetail.Description : string.Empty,
                PhidgetTypes = availablePhidgetTypes,
                PhidgetStyleTypes = availablePhidgetStyleTypes,
                ResponseTypes = availableResponseTypes
            };
        }

        // message queue
        public string GetMessageBody(int configId)
        {
            var config = _configsClient.GetWithDetails(configId);

            var configMessage = new ConfigMessage
            {
                Id = config.Id,
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog,
                IsDisplayActive = IsProcessRunning($"{AppSettings.Namespace}.{AppSettings.DisplayAppName}"),
                ConfigDetails = config.ConfigDetails.Select(x => new
                    ConfigDetailMessage
                    {
                        Id = x.Id,
                        ConfigId = config.Id,
                        PhidgetTypeId = x.PhidgetType.Id,
                        PhidgetStyleType = new PhidgetStyleTypeMessage
                        {
                            Id = x.PhidgetStyleType.Id,
                            IsIncremental = x.PhidgetStyleType.IsIncremental
                        },
                        ResponseType = new ResponseTypeMessage
                        {
                            Id = x.ResponseType.Id,
                            ResponseTypeCategoryId = x.ResponseType.ResponseTypeCategory.Id,
                            IsRotational = x.ResponseType.IsRotational,
                            IsUninterrupted = x.ResponseType.IsUninterrupted,
                            InteractiveActivityTypeId = x.ResponseType.InteractiveActivityType?.Id ?? 0,
                            SwfFile = x.ResponseType.InteractiveActivityType?.SwfFile ?? string.Empty
                        }
                    })
            };

            return JsonConvert.SerializeObject(configMessage);
        }

        private static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }
    }
}
