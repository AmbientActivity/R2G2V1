using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class EventLogEntryTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/EventLogEntryTypes
        [EnableQuery]
        public IQueryable<EventLogEntryType> GetEventLogEntryTypes()
        {
            return db.EventLogEntryTypes;
        }

        // GET: odata/EventLogEntryTypes(5)
        [EnableQuery]
        public SingleResult<EventLogEntryType> GetEventLogEntryType([FromODataUri] int key)
        {
            return SingleResult.Create(db.EventLogEntryTypes.Where(eventLogEntryType => eventLogEntryType.Id == key));
        }

        // PUT: odata/EventLogEntryTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<EventLogEntryType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EventLogEntryType eventLogEntryType = await db.EventLogEntryTypes.FindAsync(key);
            if (eventLogEntryType == null)
            {
                return NotFound();
            }

            patch.Put(eventLogEntryType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventLogEntryTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(eventLogEntryType);
        }

        // POST: odata/EventLogEntryTypes
        public async Task<IHttpActionResult> Post(EventLogEntryType eventLogEntryType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EventLogEntryTypes.Add(eventLogEntryType);
            await db.SaveChangesAsync();

            return Created(eventLogEntryType);
        }

        // PATCH: odata/EventLogEntryTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<EventLogEntryType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EventLogEntryType eventLogEntryType = await db.EventLogEntryTypes.FindAsync(key);
            if (eventLogEntryType == null)
            {
                return NotFound();
            }

            patch.Patch(eventLogEntryType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventLogEntryTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(eventLogEntryType);
        }

        // DELETE: odata/EventLogEntryTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            EventLogEntryType eventLogEntryType = await db.EventLogEntryTypes.FindAsync(key);
            if (eventLogEntryType == null)
            {
                return NotFound();
            }

            db.EventLogEntryTypes.Remove(eventLogEntryType);
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

        private bool EventLogEntryTypeExists(int key)
        {
            return db.EventLogEntryTypes.Count(e => e.Id == key) > 0;
        }
    }
}
