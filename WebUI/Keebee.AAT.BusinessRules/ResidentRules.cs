using System.Collections.Generic;
using Keebee.AAT.RESTClient;

namespace Keebee.AAT.BusinessRules
{
    public class ResidentRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        // validation
        public List<string> Validate(string firstName, string lastName, string gender, bool addnew)
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
