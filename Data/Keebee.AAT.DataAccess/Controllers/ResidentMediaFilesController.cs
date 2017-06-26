using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ResidentMediaFilesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ResidentMediaFiles
        [EnableQuery]
        public IQueryable<ResidentMediaFile> GetResidentMediaFiles()
        {
            return db.ResidentMediaFiles;
        }

        // GET: odata/ResidentMediaFiles(5)
        [EnableQuery]
        public SingleResult<ResidentMediaFile> GetResidentMediaFile([FromODataUri] int key)
        {
            return SingleResult.Create(db.ResidentMediaFiles.Where(residentMediaFile => residentMediaFile.Id == key));
        }

        // PUT: odata/ResidentMediaFiles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ResidentMediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResidentMediaFile residentMediaFile = await db.ResidentMediaFiles.FindAsync(key);
            if (residentMediaFile == null)
            {
                return NotFound();
            }

            patch.Put(residentMediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResidentMediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(residentMediaFile);
        }

        // POST: odata/ResidentMediaFiles
        public async Task<IHttpActionResult> Post(ResidentMediaFile residentMediaFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ResidentMediaFiles.Add(residentMediaFile);
            await db.SaveChangesAsync();

            return Created(residentMediaFile);
        }

        // PATCH: odata/ResidentMediaFiles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ResidentMediaFile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResidentMediaFile residentMediaFile = await db.ResidentMediaFiles.FindAsync(key);
            if (residentMediaFile == null)
            {
                return NotFound();
            }

            patch.Patch(residentMediaFile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResidentMediaFileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(residentMediaFile);
        }

        // DELETE: odata/ResidentMediaFiles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ResidentMediaFile residentMediaFile = await db.ResidentMediaFiles.FindAsync(key);
            if (residentMediaFile == null)
            {
                return NotFound();
            }

            db.ResidentMediaFiles.Remove(residentMediaFile);
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

        private bool ResidentMediaFileExists(int key)
        {
            return db.ResidentMediaFiles.Count(e => e.Id == key) > 0;
        }
    }
}
