using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class PhidgetTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/PhidgetTypes
        [EnableQuery]
        public IQueryable<PhidgetType> GetPhidgetTypes()
        {
            return db.PhidgetTypes;
        }

        // GET: odata/PhidgetTypes(5)
        [EnableQuery]
        public SingleResult<PhidgetType> GetPhidgetType([FromODataUri] int key)
        {
            return SingleResult.Create(db.PhidgetTypes.Where(phidgetType => phidgetType.Id == key));
        }

        // PUT: odata/PhidgetTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<PhidgetType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PhidgetType phidgetType = await db.PhidgetTypes.FindAsync(key);
            if (phidgetType == null)
            {
                return NotFound();
            }

            patch.Put(phidgetType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhidgetTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(phidgetType);
        }

        // POST: odata/PhidgetTypes
        public async Task<IHttpActionResult> Post(PhidgetType phidgetType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PhidgetTypes.Add(phidgetType);
            await db.SaveChangesAsync();

            return Created(phidgetType);
        }

        // PATCH: odata/PhidgetTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<PhidgetType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PhidgetType phidgetType = await db.PhidgetTypes.FindAsync(key);
            if (phidgetType == null)
            {
                return NotFound();
            }

            patch.Patch(phidgetType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhidgetTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(phidgetType);
        }

        // DELETE: odata/PhidgetTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            PhidgetType phidgetType = await db.PhidgetTypes.FindAsync(key);
            if (phidgetType == null)
            {
                return NotFound();
            }

            db.PhidgetTypes.Remove(phidgetType);
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

        private bool PhidgetTypeExists(int key)
        {
            return db.PhidgetTypes.Count(e => e.Id == key) > 0;
        }
    }
}
