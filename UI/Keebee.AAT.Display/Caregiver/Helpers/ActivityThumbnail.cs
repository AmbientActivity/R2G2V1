using System;
using System.Drawing;
using Keebee.AAT.Shared;

namespace Keebee.AAT.Display.Caregiver.Helpers
{
    public static class ActivityThumbnail
    {
        public static Image Get(int activityTypeId)
        {
            switch (activityTypeId)
            {
                case InteractiveActivityTypeId.MatchingGame:
                    return
                        ThumbnailHelper.GetImageFromByteArray(Convert.FromBase64String(ImagesBase64.MatchingGameThumbnail));
                case InteractiveActivityTypeId.PaintingActivity:
                    return
                        ThumbnailHelper.GetImageFromByteArray(Convert.FromBase64String(ImagesBase64.PaintingActivityThumbnail));
                case InteractiveActivityTypeId.BalloonPoppingGame:
                    return
                        ThumbnailHelper.GetImageFromByteArray(Convert.FromBase64String(ImagesBase64.BalloonPoppingGameThumbnail));
                default:
                    return null;
            }
        }
    }
}
