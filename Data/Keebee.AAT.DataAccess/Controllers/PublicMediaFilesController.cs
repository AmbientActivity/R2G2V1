using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class PublicMediaFilesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/PublicMediaFiles
        [EnableQuery]
        public IQueryable<PublicMediaFile> GetPublicMediaFiles()
        {
            return db.PublicMediaFiles;
        }

        // GET: odata/PublicMediaFiles(5)
        [EnableQuery]
        public SingleResult<PublicMediaFile> GetPublicMediaFile([FromODataUri] int key)
        {
            return SingleResult.Create(db.PublicMediaFiles.Where(publicMediaFile => publicMediaFile.Id == key));
        }

        // PUT: odata/PublicMediaFiles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<PublicMediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PublicMediaFile publicMediaFile = await db.PublicMediaFiles.FindAsync(key);
            if (publicMediaFile == null)
            {
                return NotFound();
            }

            patch.Put(publicMediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicMediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(publicMediaFile);
        }

        // POST: odata/PublicMediaFiles
        public async Task<IHttpActionResult> Post(PublicMediaFile publicMediaFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PublicMediaFiles.Add(publicMediaFile);
            await db.SaveChangesAsync();

            return Created(publicMediaFile);
        }

        // PATCH: odata/PublicMediaFiles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<PublicMediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PublicMediaFile publicMediaFile = await db.PublicMediaFiles.FindAsync(key);
            if (publicMediaFile == null)
            {
                return NotFound();
            }

            patch.Patch(publicMediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicMediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(publicMediaFile);
        }

        // DELETE: odata/PublicMediaFiles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            PublicMediaFile publicMediaFile = await db.PublicMediaFiles.FindAsync(key);
            if (publicMediaFile == null)
            {
                return NotFound();
            }

            db.PublicMediaFiles.Remove(publicMediaFile);
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

        private bool PublicMediaFileExists(int key)
        {
            return db.PublicMediaFiles.Count(e => e.Id == key) > 0;
        }
    }
}
