using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ResponseTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ResponseTypes
        [EnableQuery]
        public IQueryable<ResponseType> GetResponseTypes()
        {
            return db.ResponseTypes.OrderBy(o => o.Id);
        }

        // GET: odata/ResponseTypes(5)
        [EnableQuery]
        public SingleResult<ResponseType> GetResponseType([FromODataUri] int key)
        {
            return SingleResult.Create(db.ResponseTypes.Where(responseType => responseType.Id == key));
        }

        // PUT: odata/ResponseTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ResponseType> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResponseType responseType = await db.ResponseTypes.FindAsync(key);
            if (responseType == null)
            {
                return NotFound();
            }

            patch.Put(responseType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResponseTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(responseType);
        }

        // POST: odata/ResponseTypes
        public async Task<IHttpActionResult> Post(ResponseType responseType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ResponseTypes.Add(responseType);
            await db.SaveChangesAsync();

            return Created(responseType);
        }

        // PATCH: odata/ResponseTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ResponseType> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResponseType responseType = await db.ResponseTypes.FindAsync(key);
            if (responseType == null)
            {
                return NotFound();
            }

            patch.Patch(responseType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResponseTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(responseType);
        }

        // DELETE: odata/ResponseTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ResponseType responseType = await db.ResponseTypes.FindAsync(key);
            if (responseType == null)
            {
                return NotFound();
            }

            db.ResponseTypes.Remove(responseType);
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

        private bool ResponseTypeExists(int key)
        {
            return db.ResponseTypes.Count(e => e.Id == key) > 0;
        }
    }
}
