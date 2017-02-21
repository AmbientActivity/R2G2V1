using Keebee.AAT.Shared;
using System.Collections.Generic;

namespace Keebee.AAT.Display.Helpers
{
    public class AmbientInvitationMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int ResponseTypeId { get; set; }
    }

    public static class AmbientInvitationMessages
    {
        public static IList<AmbientInvitationMessage> Load(string[] messages, int[]responseTypeIds)
        {
            var invitationMessages = new List<AmbientInvitationMessage>();
            var messageCount = messages.Length;

            for(var i = 0; i < messageCount; i++)
            {
                if (messages[i].Length > 0)
                    invitationMessages.Add(new AmbientInvitationMessage
                    {
                        Id = 1,
                        Message = messages[i],
                        ResponseTypeId = ValidateResponseType(responseTypeIds[i])
                    });
            }

            return invitationMessages;
        }

        private static int ValidateResponseType(int responseTypeId)
        {
            switch (responseTypeId)
            {
                case ResponseTypeId.SlideShow:
                    return ResponseTypeId.SlideShow;
                case ResponseTypeId.MatchingGame:
                    return ResponseTypeId.MatchingGame;
                case ResponseTypeId.Cats:
                    return ResponseTypeId.Cats;
                case ResponseTypeId.Radio:
                    return ResponseTypeId.Radio;
                case ResponseTypeId.Television:
                    return ResponseTypeId.Television;
                default:
                    return 0;
            }
        }
    }
}
