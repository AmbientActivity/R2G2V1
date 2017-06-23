using Keebee.AAT.Operations.Helpers;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/audio")]
    public class AudioController : ApiController
    {
        private readonly IMediaFileService _mediaFileService;

        public AudioController(IMediaFileService mediaFileService)
        {
            _mediaFileService = mediaFileService;
        }

        public HttpResponseMessage Get(Guid id)
        {
            var file = _mediaFileService.Get(id);
            var audio = new AudioStreamer($@"{file.Path}\{file.Filename}");
            var fileType = file.FileType.ToLower();

            var response = Request.CreateResponse();
            response.Content = new PushStreamContent(audio.WriteToStream, new MediaTypeHeaderValue($"audio/{fileType}"));

            return response;
        }
    }
}
