using System;
using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class GamingEventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/GamingEventLogs
        [EnableQuery]
        public IQueryable<GamingEventLog> GetGamingEventLogs()
        {
            return db.GamingEventLogs;
        }

        // GET: odata/GamingEventLogs(5)
        [EnableQuery]
        public SingleResult<GamingEventLog> GetGamingEventLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.GamingEventLogs.Where(gamingEventLog => gamingEventLog.Id == key));
        }

        // PUT: odata/GamingEventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<GamingEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GamingEventLog gamingEventLog = await db.GamingEventLogs.FindAsync(key);
            if (gamingEventLog == null)
            {
                return NotFound();
            }

            patch.Put(gamingEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GamingEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gamingEventLog);
        }

        // POST: odata/GamingEventLogs
        public async Task<IHttpActionResult> Post(GamingEventLog gamingEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            gamingEventLog.DateEntry = DateTime.Now;
            db.GamingEventLogs.Add(gamingEventLog);
            await db.SaveChangesAsync();

            return Created(gamingEventLog);
        }

        // PATCH: odata/GamingEventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<GamingEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GamingEventLog gamingEventLog = await db.GamingEventLogs.FindAsync(key);
            if (gamingEventLog == null)
            {
                return NotFound();
            }

            patch.Patch(gamingEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GamingEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gamingEventLog);
        }

        // DELETE: odata/GamingEventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            GamingEventLog gamingEventLog = await db.GamingEventLogs.FindAsync(key);
            if (gamingEventLog == null)
            {
                return NotFound();
            }

            db.GamingEventLogs.Remove(gamingEventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/GamingEventLogs(5)/EventLogEntryType
        [EnableQuery]
        public SingleResult<EventLogEntryType> GetEventLogEntryType([FromODataUri] int key)
        {
            return SingleResult.Create(db.GamingEventLogs.Where(m => m.Id == key).Select(m => m.EventLogEntryType));
        }

        // GET: odata/GamingEventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.GamingEventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GamingEventLogExists(int key)
        {
            return db.GamingEventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
