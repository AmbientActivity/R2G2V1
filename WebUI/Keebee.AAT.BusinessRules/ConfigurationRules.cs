using Keebee.AAT.RESTClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Keebee.AAT.BusinessRules.DTO;

namespace Keebee.AAT.BusinessRules
{
    public class ConfigurationRules
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

            if (config.Id != 0)
                msgs.Add($"A configuration with the name '{description}' already exists");

            return msgs.Count > 0 ? msgs : null;
        }

        public List<string> ValidateDetail(string description)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Description is required");

            return msgs.Count > 0 ? msgs : null;
        }

        // view model
        public ConfigEditModel GetConfigEditViewModel(int id, int configId)
        {
            ConfigDetail configDetail = null;
            IEnumerable<int> usedPhidgetIds;
            IEnumerable<ResponseType> availableResponseTypes;
            IEnumerable<PhidgetType> availablePhidgetTypes;
            var allPhidgetTypes = _opsClient.GetPhidgetTypes().ToArray();

            // edit mode
            if (id > 0)
            {
                var config = _opsClient.GetConfig(configId);
                configDetail = _opsClient.GetConfigDetail(id);

                // acive config - don't allow modification of phidget or reponse type
                if (config.IsActive)
                {
                    availableResponseTypes = new Collection<ResponseType>()
                                    {
                                        new ResponseType
                                        {
                                            Id = configDetail.ResponseType.Id,
                                            Description = configDetail.ResponseType.Description
                                        }
                                    };

                    usedPhidgetIds = allPhidgetTypes
                        .Where(pt => pt.Id != configDetail.PhidgetType.Id)
                        .Select(pt => pt.Id);

                    availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();
                }
                else // non-active config
                {
                    // get all response types
                    availableResponseTypes = _opsClient.GetResponseTypes();

                    // get all unused phidgets plus the one being edited
                    usedPhidgetIds = _opsClient.GetConfigWithDetails(configDetail.ConfigId)
                        .ConfigDetails
                        .Where(cd => cd.PhidgetType.Id != configDetail.PhidgetType.Id)
                        .Select(cd => cd.PhidgetType.Id);

                    availablePhidgetTypes = allPhidgetTypes.Where(pt => !usedPhidgetIds.Contains(pt.Id)).ToArray();
                }
            }
            else // add mode
            {
                // get all response types
                availableResponseTypes = _opsClient.GetResponseTypes();

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
                ResponseTypes = availableResponseTypes
            };
        }
    }
}
