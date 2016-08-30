using Keebee.AAT.DataAccess.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class MediaFilesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/MediaFiles
        [EnableQuery]
        public IQueryable<MediaFile> GetMediaFiles()
        {
            return db.MediaFiles;
        }

        // GET: odata/MediaFiles(5)
        [EnableQuery]
        public SingleResult<MediaFile> GetMediaFile([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.MediaFiles.Where(mediaFile => mediaFile.StreamId == key));
        }

        // PUT: odata/MediaFiles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<MediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MediaFile mediaFile = await db.MediaFiles.FindAsync(key);
            if (mediaFile == null)
            {
                return NotFound();
            }

            patch.Put(mediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(mediaFile);
        }

        // POST: odata/MediaFiles
        public async Task<IHttpActionResult> Post(MediaFile mediaFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MediaFiles.Add(mediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MediaFileExists(mediaFile.StreamId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(mediaFile);
        }

        // PATCH: odata/MediaFiles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<MediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MediaFile mediaFile = await db.MediaFiles.FindAsync(key);
            if (mediaFile == null)
            {
                return NotFound();
            }

            patch.Patch(mediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(mediaFile);
        }

        // DELETE: odata/MediaFiles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            MediaFile mediaFile = await db.MediaFiles.FindAsync(key);
            if (mediaFile == null)
            {
                return NotFound();
            }

            db.MediaFiles.Remove(mediaFile);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MediaFileExists(Guid key)
        {
            return db.MediaFiles.Count(e => e.StreamId == key) > 0;
        }
    }
}
