using System.Collections.Generic;

namespace Keebee.AAT.BusinessRules
{
    public static class ValidationRules
    {
        // config
        public static List<string> ValidateConfig(string description)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Description is required");

            return msgs.Count > 0 ? msgs : null;
        }

        // config detail
        public static List<string> ValidateConfigDetail(string description)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(description))
                msgs.Add("Description is required");

            return msgs.Count > 0 ? msgs : null;
        }

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
