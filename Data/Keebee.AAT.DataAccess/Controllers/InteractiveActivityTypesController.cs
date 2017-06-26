using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class InteractiveActivityTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/InteractiveActivityTypes
        [EnableQuery]
        public IQueryable<InteractiveActivityType> GetInteractiveActivityTypes()
        {
            return db.InteractiveActivityTypes;
        }

        // GET: odata/InteractiveActivityTypes(5)
        [EnableQuery]
        public SingleResult<InteractiveActivityType> GetInteractiveActivityType([FromODataUri] int key)
        {
            return SingleResult.Create(db.InteractiveActivityTypes.Where(gameType => gameType.Id == key));
        }

        // PUT: odata/InteractiveActivityTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<InteractiveActivityType> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InteractiveActivityType gameType = await db.InteractiveActivityTypes.FindAsync(key);
            if (gameType == null)
            {
                return NotFound();
            }

            patch.Put(gameType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InteractiveActivityTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gameType);
        }

        // POST: odata/InteractiveActivityTypes
        public async Task<IHttpActionResult> Post(InteractiveActivityType gameType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.InteractiveActivityTypes.Add(gameType);
            await db.SaveChangesAsync();

            return Created(gameType);
        }

        // PATCH: odata/InteractiveActivityTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<InteractiveActivityType> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InteractiveActivityType gameType = await db.InteractiveActivityTypes.FindAsync(key);
            if (gameType == null)
            {
                return NotFound();
            }

            patch.Patch(gameType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InteractiveActivityTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gameType);
        }

        // DELETE: odata/InteractiveActivityTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            InteractiveActivityType gameType = await db.InteractiveActivityTypes.FindAsync(key);
            if (gameType == null)
            {
                return NotFound();
            }

            db.InteractiveActivityTypes.Remove(gameType);
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

        private bool InteractiveActivityTypeExists(int key)
        {
            return db.InteractiveActivityTypes.Count(e => e.Id == key) > 0;
        }
    }
}
