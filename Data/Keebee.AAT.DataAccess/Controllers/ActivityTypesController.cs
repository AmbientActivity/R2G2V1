using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ActivityTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ActivityTypes
        [EnableQuery]
        public IQueryable<ActivityType> GetActivityTypes()
        {
            return db.ActivityTypes;
        }

        // GET: odata/ActivityTypes(5)
        [EnableQuery]
        public SingleResult<ActivityType> GetActivityType([FromODataUri] int key)
        {
            return SingleResult.Create(db.ActivityTypes.Where(activityType => activityType.Id == key));
        }

        // PUT: odata/ActivityTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ActivityType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActivityType activityType = await db.ActivityTypes.FindAsync(key);
            if (activityType == null)
            {
                return NotFound();
            }

            patch.Put(activityType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activityType);
        }

        // POST: odata/ActivityTypes
        public async Task<IHttpActionResult> Post(ActivityType activityType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ActivityTypes.Add(activityType);
            await db.SaveChangesAsync();

            return Created(activityType);
        }

        // PATCH: odata/ActivityTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ActivityType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActivityType activityType = await db.ActivityTypes.FindAsync(key);
            if (activityType == null)
            {
                return NotFound();
            }

            patch.Patch(activityType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activityType);
        }

        // DELETE: odata/ActivityTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ActivityType activityType = await db.ActivityTypes.FindAsync(key);
            if (activityType == null)
            {
                return NotFound();
            }

            db.ActivityTypes.Remove(activityType);
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

        private bool ActivityTypeExists(int key)
        {
            return db.ActivityTypes.Count(e => e.Id == key) > 0;
        }
    }
}
