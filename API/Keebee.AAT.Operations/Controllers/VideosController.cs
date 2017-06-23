using Keebee.AAT.Operations.Helpers;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/videos")]
    public class VideosController : ApiController
    {
        private readonly IMediaFileService _mediaFileService;
        public static IReadOnlyDictionary<string, string> MimeNames;

        public VideosController(IMediaFileService mediaFileService)
        {
            _mediaFileService = mediaFileService;

            var mimeNames = new Dictionary<string, string>
            {
                {".mp3", "audio/mpeg"},
                {".mp4", "video/mp4"},
                {".ogg", "application/ogg"},
                {".ogv", "video/ogg"},
                {".oga", "audio/ogg"},
                {".wav", "audio/x-wav"},
                {".webm", "video/webm"}
            };

            // List all supported media types; 

            MimeNames = new ReadOnlyDictionary<string, string>(mimeNames);

            //InvalidFileNameChars = Array.AsReadOnly(Path.GetInvalidFileNameChars());
        }

        public HttpResponseMessage Get(Guid id)
        {
            var file = _mediaFileService.Get(id);
            var filePath = Path.Combine(file.Path, file.Filename);
            if (!File.Exists(filePath))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var response = Request.CreateResponse();
            response.Headers.AcceptRanges.Add("bytes");

            var streamer = new VideoStreamer {FileInfo = new FileInfo(filePath)};
            response.Content = new PushStreamContent(streamer.WriteToStream, GetMimeType($".{file.FileType}"));

            RangeHeaderValue rangeHeader = Request.Headers.Range;
            if (rangeHeader != null)
            {
                long totalLength = streamer.FileInfo.Length;
                var range = rangeHeader.Ranges.First();
                streamer.Start = range.From ?? 0;
                streamer.End = range.To ?? totalLength - 1;

                response.Content.Headers.ContentLength = streamer.End - streamer.Start + 1;
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(streamer.Start, streamer.End,
                    totalLength);
                response.StatusCode = HttpStatusCode.PartialContent;
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }

        private static MediaTypeHeaderValue GetMimeType(string ext)
        {
            string value;

            return MimeNames.TryGetValue(ext.ToLowerInvariant(), out value) 
                ? new MediaTypeHeaderValue(value) 
                : new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
        }
    }
}
