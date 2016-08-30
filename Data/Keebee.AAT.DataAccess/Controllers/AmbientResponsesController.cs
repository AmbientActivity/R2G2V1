using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class AmbientResponsesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/AmbientResponses
        [EnableQuery]
        public IQueryable<AmbientResponse> GetAmbientResponses()
        {
            return db.AmbientResponses.OrderBy(o => o.Id);
        }

        // GET: odata/AmbientResponses(5)
        [EnableQuery]
        public SingleResult<AmbientResponse> GetAmbientResponse([FromODataUri] int key)
        {
            return SingleResult.Create(db.AmbientResponses.Where(response => response.Id == key));
        }

        // PUT: odata/AmbientResponses(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<AmbientResponse> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AmbientResponse response = await db.AmbientResponses.FindAsync(key);
            if (response == null)
            {
                return NotFound();
            }

            patch.Put(response);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AmbientResponseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(response);
        }

        // POST: odata/AmbientResponse
        public async Task<IHttpActionResult> Post(AmbientResponse response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AmbientResponses.Add(response);
            await db.SaveChangesAsync();

            return Created(response);
        }

        // PATCH: odata/AmbientResponses(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<AmbientResponse> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AmbientResponse response = await db.AmbientResponses.FindAsync(key);
            if (response == null)
            {
                return NotFound();
            }

            patch.Patch(response);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AmbientResponseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(response);
        }

        // DELETE: odata/AmbientResponses(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            AmbientResponse response = await db.AmbientResponses.FindAsync(key);
            if (response == null)
            {
                return NotFound();
            }

            db.AmbientResponses.Remove(response);
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

        private bool AmbientResponseExists(int key)
        {
            return db.AmbientResponses.Count(e => e.Id == key) > 0;
        }
    }
}
