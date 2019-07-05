using System.Collections.Generic;

namespace Keebee.AAT.BusinessRules
{
    public class AmbientInvitationRules
    {
        // validation
        public static List<string> Validate(string message)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(message))
                msgs.Add("Message is required");

            return msgs.Count > 0 ? msgs : null;
        }
    }
}
