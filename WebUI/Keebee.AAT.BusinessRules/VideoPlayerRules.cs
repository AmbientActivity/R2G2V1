using Keebee.AAT.BusinessRules.Models;
using System;

namespace Keebee.AAT.BusinessRules
{
    public class VideoPlayerRules
    {
        public VideoPlayerModel GetVideoPlayerModel(Guid streamId)
        {
            return new VideoPlayerModel
            {
                Url = "http://localhost/Keebee.AAT.Operations/api/videos?streamId=" + streamId
            };
        }
    }
}