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
    public class ActivityEventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ActivityEventLogs
        [EnableQuery]
        public IQueryable<ActivityEventLog> Get()
        {
            return db.ActivityEventLogs;
        }

        // GET: odata/ActivityEventLogs(5)
        [EnableQuery]
        public SingleResult<ActivityEventLog> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.ActivityEventLogs.Where(activityEventLog => activityEventLog.Id == key));
        }

        // PUT: odata/ActivityEventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ActivityEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActivityEventLog activityEventLog = await db.ActivityEventLogs.FindAsync(key);
            if (activityEventLog == null)
            {
                return NotFound();
            }

            patch.Put(activityEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activityEventLog);
        }

        // POST: odata/ActivityEventLogs
        public async Task<IHttpActionResult> Post(ActivityEventLog activityEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            activityEventLog.DateEntry = DateTime.Now;
            db.ActivityEventLogs.Add(activityEventLog);
            await db.SaveChangesAsync();

            return Created(activityEventLog);
        }

        // PATCH: odata/ActivityEventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ActivityEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActivityEventLog activityEventLog = await db.ActivityEventLogs.FindAsync(key);
            if (activityEventLog == null)
            {
                return NotFound();
            }

            patch.Patch(activityEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activityEventLog);
        }

        // DELETE: odata/ActivityEventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ActivityEventLog activityEventLog = await db.ActivityEventLogs.FindAsync(key);
            if (activityEventLog == null)
            {
                return NotFound();
            }

            db.ActivityEventLogs.Remove(activityEventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ActivityEventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.ActivityEventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityEventLogExists(int key)
        {
            return db.ActivityEventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
