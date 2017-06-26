using System;
using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ThumbnailsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/Thumbnails
        [EnableQuery]
        public IQueryable<Thumbnail> GetThumbnails()
        {
            return db.Thumbnails;
        }

        // GET: odata/Thumbnails(5)
        [EnableQuery]
        public SingleResult<Thumbnail> GetThumbnail([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Thumbnails.Where(thumbnail => thumbnail.StreamId == key));
        }

        // PUT: odata/Thumbnails(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<Thumbnail> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Thumbnail thumbnail = await db.Thumbnails.FindAsync(key);
            if (thumbnail == null)
            {
                return NotFound();
            }

            patch.Put(thumbnail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThumbnailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(thumbnail);
        }

        // POST: odata/Thumbnails
        public async Task<IHttpActionResult> Post(Thumbnail thumbnail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Thumbnails.Add(thumbnail);
            await db.SaveChangesAsync();

            return Created(thumbnail);
        }

        // PATCH: odata/Thumbnails(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Thumbnail> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Thumbnail thumbnail = await db.Thumbnails.FindAsync(key);
            if (thumbnail == null)
            {
                return NotFound();
            }

            patch.Patch(thumbnail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThumbnailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(thumbnail);
        }

        // DELETE: odata/Thumbnails(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            Thumbnail thumbnail = await db.Thumbnails.FindAsync(key);
            if (thumbnail == null)
            {
                return NotFound();
            }

            db.Thumbnails.Remove(thumbnail);
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

        private bool ThumbnailExists(Guid key)
        {
            return db.Thumbnails.Count(e => e.StreamId == key) > 0;
        }
    }
}
