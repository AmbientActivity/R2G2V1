using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ResponsesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/Response
        [EnableQuery]
        public IQueryable<Response> GetResponses()
        {
            return db.Responses;
        }

        // GET: odata/Responses(5)
        [EnableQuery]
        public SingleResult<Response> GetResponse([FromODataUri] int key)
        {
            return SingleResult.Create(db.Responses.Where(response => response.Id == key));
        }

        // PUT: odata/Response(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Response> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Response response = await db.Responses.FindAsync(key);
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
                if (!ResponseExists(key))
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

        // POST: odata/Response
        public async Task<IHttpActionResult> Post(Response response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Responses.Add(response);
            await db.SaveChangesAsync();

            return Created(response);
        }

        // PATCH: odata/Response(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Response> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Response response = await db.Responses.FindAsync(key);
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
                if (!ResponseExists(key))
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

        // DELETE: odata/Response(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Response response = await db.Responses.FindAsync(key);
            if (response == null)
            {
                return NotFound();
            }

            db.Responses.Remove(response);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Response(5)/MediaFile
        [EnableQuery]
        public SingleResult<MediaFile> GetMediaFile([FromODataUri] int key)
        {
            return SingleResult.Create(db.Responses.Where(m => m.Id == key).Select(m => m.MediaFile));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ResponseExists(int key)
        {
            return db.Responses.Count(e => e.Id == key) > 0;
        }
    }
}
