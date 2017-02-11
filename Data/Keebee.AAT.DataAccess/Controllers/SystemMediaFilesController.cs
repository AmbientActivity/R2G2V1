using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class SystemMediaFilesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/SystemMediaFiles
        [EnableQuery]
        public IQueryable<SystemMediaFile> GetSystemMediaFiles()
        {
            return db.SystemMediaFiles;
        }

        // GET: odata/SystemMediaFiles(5)
        [EnableQuery]
        public SingleResult<SystemMediaFile> GetSystemMediaFile([FromODataUri] int key)
        {
            return SingleResult.Create(db.SystemMediaFiles.Where(systemMediaFile => systemMediaFile.Id == key));
        }

        // PUT: odata/SystemMediaFiles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SystemMediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SystemMediaFile systemMediaFile = await db.SystemMediaFiles.FindAsync(key);
            if (systemMediaFile == null)
            {
                return NotFound();
            }

            patch.Put(systemMediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemMediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(systemMediaFile);
        }

        // POST: odata/SystemMediaFiles
        public async Task<IHttpActionResult> Post(SystemMediaFile systemMediaFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SystemMediaFiles.Add(systemMediaFile);
            await db.SaveChangesAsync();

            return Created(systemMediaFile);
        }

        // PATCH: odata/SystemMediaFiles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SystemMediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SystemMediaFile systemMediaFile = await db.SystemMediaFiles.FindAsync(key);
            if (systemMediaFile == null)
            {
                return NotFound();
            }

            patch.Patch(systemMediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemMediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(systemMediaFile);
        }

        // DELETE: odata/SystemMediaFiles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SystemMediaFile systemMediaFile = await db.SystemMediaFiles.FindAsync(key);
            if (systemMediaFile == null)
            {
                return NotFound();
            }

            db.SystemMediaFiles.Remove(systemMediaFile);
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

        private bool SystemMediaFileExists(int key)
        {
            return db.SystemMediaFiles.Count(e => e.Id == key) > 0;
        }
    }
}
