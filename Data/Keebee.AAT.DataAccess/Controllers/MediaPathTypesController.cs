using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class MediaPathTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/MediaPathTypes
        [EnableQuery]
        public IQueryable<MediaPathType> GetMediaPathTypes()
        {
            return db.MediaPathTypes;
        }

        // GET: odata/MediaPathTypes(5)
        [EnableQuery]
        public SingleResult<MediaPathType> GetMediaPathType([FromODataUri] int key)
        {
            return SingleResult.Create(db.MediaPathTypes.Where(mediaPathType => mediaPathType.Id == key));
        }

        // PUT: odata/MediaPathTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<MediaPathType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MediaPathType mediaPathType = await db.MediaPathTypes.FindAsync(key);
            if (mediaPathType == null)
            {
                return NotFound();
            }

            patch.Put(mediaPathType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaPathTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(mediaPathType);
        }

        // POST: odata/MediaPathTypes
        public async Task<IHttpActionResult> Post(MediaPathType mediaPathType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MediaPathTypes.Add(mediaPathType);
            await db.SaveChangesAsync();

            return Created(mediaPathType);
        }

        // PATCH: odata/MediaPathTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<MediaPathType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MediaPathType mediaPathType = await db.MediaPathTypes.FindAsync(key);
            if (mediaPathType == null)
            {
                return NotFound();
            }

            patch.Patch(mediaPathType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaPathTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(mediaPathType);
        }

        // DELETE: odata/MediaPathTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            MediaPathType mediaPathType = await db.MediaPathTypes.FindAsync(key);
            if (mediaPathType == null)
            {
                return NotFound();
            }

            db.MediaPathTypes.Remove(mediaPathType);
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

        private bool MediaPathTypeExists(int key)
        {
            return db.MediaPathTypes.Count(e => e.Id == key) > 0;
        }
    }
}
