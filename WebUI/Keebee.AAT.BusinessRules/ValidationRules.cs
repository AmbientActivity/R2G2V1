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
                msgs.Add("Name is required");

            if (!addnew) return msgs.Count > 0 ? msgs : null;

            var config = _opsClient.GetConfigByDescription(description);
            if (config.Id != 0)
                msgs.Add($"The name '{description}' already exists");

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

            if (!addnew) return msgs.Count > 0 ? msgs : null;

            var config = _opsClient.GetResidentByNameGender(firstName, lastName, gender);
            if (config.Id != 0)
            {
                var g = (gender == "M") ? "male" : "female";
                msgs.Add($"A {g} resident with the name '{firstName} {lastName}' already exists");
            }

            return msgs.Count > 0 ? msgs : null;
        }
    }
}
