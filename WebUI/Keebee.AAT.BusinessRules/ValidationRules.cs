using System.Collections.Generic;

namespace Keebee.AAT.BusinessRules
{
    public static class ValidationRules
    {
        // resident
        public static List<string> ValidateResident(string firstname, string lastname)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(firstname))
                msgs.Add("First Name is required");

            if (string.IsNullOrEmpty(lastname))
                msgs.Add("Last Name is required");

            return msgs.Count > 0 ? msgs : null;
        }
    }
}
