using System.Collections.Generic;
using Keebee.AAT.RESTClient;

namespace Keebee.AAT.BusinessRules
{
    public class ValidationRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        // config
        public List<string> ValidateConfig(string description, bool addnew)
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

        // config detail
        public List<string> ValidateConfigDetail(string description)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Description is required");

            return msgs.Count > 0 ? msgs : null;
        }

        // resident
        public List<string> ValidateResident(string firstName, string lastName, string gender, bool addnew)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(firstName))
                msgs.Add("First Name is required");

            if (string.IsNullOrEmpty(gender))
                msgs.Add("Gender is required");

            if (!addnew) return msgs.Count > 0 ? msgs : null;

            var resident = _opsClient.GetResidentByNameGender(firstName, lastName, gender);
            if (resident == null) return msgs.Count > 0 ? msgs : null;
            if (resident.Id == 0) return msgs.Count > 0 ? msgs : null;

            var g = (gender == "M") ? "male" : "female";
            var residentName = (lastName.Length > 0) ? $"{firstName} {lastName}" : firstName;

            msgs.Add($"A {g} resident with the name '{residentName}' already exists");

            return msgs.Count > 0 ? msgs : null;
        }
    }
}
