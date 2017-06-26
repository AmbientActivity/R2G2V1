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
    public class ActiveResidentEventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ActiveResidentEventLogs
        [EnableQuery]
        public IQueryable<ActiveResidentEventLog> GetActiveResidentEventLogs()
        {
            return db.ActiveResidentEventLogs;
        }

        // GET: odata/ActiveResidentEventLogs(5)
        [EnableQuery]
        public SingleResult<ActiveResidentEventLog> GetActiveResidentEventLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.ActiveResidentEventLogs.Where(activeResidentEventLog => activeResidentEventLog.Id == key));
        }

        // PUT: odata/ActiveResidentEventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ActiveResidentEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActiveResidentEventLog activeResidentEventLog = await db.ActiveResidentEventLogs.FindAsync(key);
            if (activeResidentEventLog == null)
            {
                return NotFound();
            }

            patch.Put(activeResidentEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActiveResidentEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activeResidentEventLog);
        }

        // POST: odata/ActiveResidentEventLogs
        public async Task<IHttpActionResult> Post(ActiveResidentEventLog activeResidentEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            activeResidentEventLog.DateEntry = DateTime.Now;
            db.ActiveResidentEventLogs.Add(activeResidentEventLog);
            await db.SaveChangesAsync();

            return Created(activeResidentEventLog);
        }

        // PATCH: odata/ActiveResidentEventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ActiveResidentEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActiveResidentEventLog activeResidentEventLog = await db.ActiveResidentEventLogs.FindAsync(key);
            if (activeResidentEventLog == null)
            {
                return NotFound();
            }

            patch.Patch(activeResidentEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActiveResidentEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activeResidentEventLog);
        }

        // DELETE: odata/ActiveResidentEventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ActiveResidentEventLog activeResidentEventLog = await db.ActiveResidentEventLogs.FindAsync(key);
            if (activeResidentEventLog == null)
            {
                return NotFound();
            }

            db.ActiveResidentEventLogs.Remove(activeResidentEventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ActiveResidentEventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.ActiveResidentEventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActiveResidentEventLogExists(int key)
        {
            return db.ActiveResidentEventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
