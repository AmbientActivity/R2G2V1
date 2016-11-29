﻿using Keebee.AAT.RESTClient;
using Keebee.AAT.BusinessRules.DTO;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;

namespace Keebee.AAT.BusinessRules
{
    public class ConfigRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        // validation
        public List<string> Validate(string description, bool addnew)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Configuration name is required");

            if (!addnew) return msgs.Count > 0 ? msgs : null;
            if (description?.Length == 0) return msgs;

            var config = _opsClient.GetConfigByDescription(description);

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
                    if (phidgetStyleTypeId == PhidgetStyleTypeIdId.OnOff)
                        msgs.Add("Sensors cannot be of type On/Off");
                    break;
                case PhidgetTypeId.Input0:
                case PhidgetTypeId.Input1:
                case PhidgetTypeId.Input2:
                case PhidgetTypeId.Input3:
                case PhidgetTypeId.Input4:
                case PhidgetTypeId.Input5:
                case PhidgetTypeId.Input6:
                case PhidgetTypeId.Input7:
                    if (phidgetStyleTypeId != PhidgetStyleTypeIdId.OnOff)
                        msgs.Add("Inputs must be of type On/Off");
                    break;
            }

            return msgs.Count > 0 ? msgs : null;
        }

        // view model
        public ConfigEditModel GetConfigEditViewModel(int id, int configId)
        {
            var availableResponseTypes = _opsClient.GetResponseTypes();
            var allPhidgetTypes = _opsClient.GetPhidgetTypes().ToArray();
            var availablePhidgetStyleTypes = _opsClient.GetPhidgetStyleTypes().ToArray();
            ConfigDetail configDetail = null;

            IEnumerable<int> usedPhidgetIds;
            IEnumerable<PhidgetType> availablePhidgetTypes;
            // edit mode
            if (id > 0)
            {
                configDetail = _opsClient.GetConfigDetail(id);

                // get all unused phidgets plus the one being edited
                usedPhidgetIds = _opsClient.GetConfigWithDetails(configDetail.ConfigId)
                    .ConfigDetails
                    .Where(cd => cd.PhidgetType.Id != configDetail.PhidgetType.Id)
                    .Select(cd => cd.PhidgetType.Id);

                availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();
            }
            else // add mode
            {
                // get only unused phidgets
                usedPhidgetIds = _opsClient.GetConfigWithDetails(configId)
                    .ConfigDetails
                    .Select(cd => cd.PhidgetType.Id);

                availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();
            }

            return new ConfigEditModel
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
            var config = _opsClient.GetConfigWithDetails(configId);

            var configMessage = new ConfigMessage
            {
                Id = config.Id,
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog,
                IsDisplayActive = IsProcessRunning(ApplicationName.DisplayApp),
                ConfigDetails = config.ConfigDetails
                    .Select(x => new
                        ConfigDetailMessage
                        {
                            Id = x.Id,
                            ConfigId = config.Id,
                            ResponseTypeId = x.ResponseType.Id,
                            PhidgetTypeId = x.PhidgetType.Id,
                            PhidgetStyleTypeId = x.PhidgetStyleType.Id
                        })
            };


            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(configMessage);

            return messageBody;
        }

        private static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }
    }
}
