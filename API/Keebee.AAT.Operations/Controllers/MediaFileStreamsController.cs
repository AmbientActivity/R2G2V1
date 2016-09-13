using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    public class MediaFileStreamsController : ApiController
    {
       private readonly IMediaFileStreamService _mediaFileStreamService;

       public MediaFileStreamsController(IMediaFileStreamService mediaService)
        {
            _mediaFileStreamService = mediaService;
        }

        // GET: api/MediaFileStreams/55a32e73-b176-e611-8a92-90e6bac7161a
        public async Task<byte[]> Get(Guid id)
        {
            var media = new MediaFileStream();

            await Task.Run(() =>
            {
                media = _mediaFileStreamService.Get(id);
            });

            return media.Stream;
        }
    }
}
