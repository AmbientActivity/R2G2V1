using Keebee.AAT.DataAccess.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class EventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/EventLogs
        [EnableQuery]
        public IQueryable<EventLog> Get()
        {
            return db.EventLogs;
        }

        // GET: odata/EventLogs(5)
        [EnableQuery]
        public SingleResult<EventLog> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.EventLogs.Where(eventLog => eventLog.Id == key));
        }

        // PUT: odata/EventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<EventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EventLog eventLog = await db.EventLogs.FindAsync(key);
            if (eventLog == null)
            {
                return NotFound();
            }

            patch.Put(eventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(eventLog);
        }

        // POST: odata/EventLogs
        public async Task<IHttpActionResult> Post(EventLog eventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            eventLog.DateEntry = DateTime.Now;
            db.EventLogs.Add(eventLog);
            await db.SaveChangesAsync();

            return Created(eventLog);
        }

        // PATCH: odata/EventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<EventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EventLog eventLog = await db.EventLogs.FindAsync(key);
            if (eventLog == null)
            {
                return NotFound();
            }

            patch.Patch(eventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(eventLog);
        }

        // DELETE: odata/EventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            EventLog eventLog = await db.EventLogs.FindAsync(key);
            if (eventLog == null)
            {
                return NotFound();
            }

            db.EventLogs.Remove(eventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/EventLogs(5)/EventLogEntryType
        [EnableQuery]
        public SingleResult<EventLogEntryType> GetEventLogEntryType([FromODataUri] int key)
        {
            return SingleResult.Create(db.EventLogs.Where(m => m.Id == key).Select(m => m.EventLogEntryType));
        }

        // GET: odata/EventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.EventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventLogExists(int key)
        {
            return db.EventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
