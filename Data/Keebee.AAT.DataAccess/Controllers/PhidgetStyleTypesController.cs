using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class PhidgetStyleTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/PhidgetStyleTypes
        [EnableQuery]
        public IQueryable<PhidgetStyleType> GetPhidgetStyleTypes()
        {
            return db.PhidgetStyleTypes;
        }

        // GET: odata/PhidgetStyleTypes(5)
        [EnableQuery]
        public SingleResult<PhidgetStyleType> GetPhidgetStyleType([FromODataUri] int key)
        {
            return SingleResult.Create(db.PhidgetStyleTypes.Where(phidgetStyleType => phidgetStyleType.Id == key));
        }

        // PUT: odata/PhidgetStyleTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<PhidgetStyleType> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PhidgetStyleType phidgetStyleType = await db.PhidgetStyleTypes.FindAsync(key);
            if (phidgetStyleType == null)
            {
                return NotFound();
            }

            patch.Put(phidgetStyleType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhidgetStyleTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(phidgetStyleType);
        }

        // POST: odata/PhidgetStyleTypes
        public async Task<IHttpActionResult> Post(PhidgetStyleType phidgetStyleType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PhidgetStyleTypes.Add(phidgetStyleType);
            await db.SaveChangesAsync();

            return Created(phidgetStyleType);
        }

        // PATCH: odata/PhidgetStyleTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<PhidgetStyleType> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PhidgetStyleType phidgetStyleType = await db.PhidgetStyleTypes.FindAsync(key);
            if (phidgetStyleType == null)
            {
                return NotFound();
            }

            patch.Patch(phidgetStyleType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhidgetStyleTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(phidgetStyleType);
        }

        // DELETE: odata/PhidgetStyleTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            PhidgetStyleType phidgetStyleType = await db.PhidgetStyleTypes.FindAsync(key);
            if (phidgetStyleType == null)
            {
                return NotFound();
            }

            db.PhidgetStyleTypes.Remove(phidgetStyleType);
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

        private bool PhidgetStyleTypeExists(int key)
        {
            return db.PhidgetStyleTypes.Count(e => e.Id == key) > 0;
        }
    }
}
