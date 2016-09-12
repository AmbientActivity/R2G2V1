using Keebee.AAT.DataAccess.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class MediaFileStreamsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/MediaFileStreams
        [EnableQuery]
        public IQueryable<MediaFileStream> GetMediaFileStreams()
        {
            return db.MediaFileStreams;
        }

        // GET: odata/MediaFileStreams(4b8bde1f-8175-e611-8a92-90e6bac7161a)
        [EnableQuery]
        public SingleResult<MediaFileStream> GetMediaFileStream([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.MediaFileStreams.Where(mediaFileStream => mediaFileStream.StreamId == key));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
