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
    public class RfidEventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/RfidEventLogs
        [EnableQuery]
        public IQueryable<RfidEventLog> GetRfidEventLogs()
        {
            return db.RfidEventLogs;
        }

        // GET: odata/RfidEventLogs(5)
        [EnableQuery]
        public SingleResult<RfidEventLog> GetRfidEventLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.RfidEventLogs.Where(rfidEventLog => rfidEventLog.Id == key));
        }

        // PUT: odata/RfidEventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<RfidEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RfidEventLog rfidEventLog = await db.RfidEventLogs.FindAsync(key);
            if (rfidEventLog == null)
            {
                return NotFound();
            }

            patch.Put(rfidEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RfidEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rfidEventLog);
        }

        // POST: odata/RfidEventLogs
        public async Task<IHttpActionResult> Post(RfidEventLog rfidEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            rfidEventLog.DateEntry = DateTime.Now;
            db.RfidEventLogs.Add(rfidEventLog);
            await db.SaveChangesAsync();

            return Created(rfidEventLog);
        }

        // PATCH: odata/RfidEventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<RfidEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RfidEventLog rfidEventLog = await db.RfidEventLogs.FindAsync(key);
            if (rfidEventLog == null)
            {
                return NotFound();
            }

            patch.Patch(rfidEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RfidEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rfidEventLog);
        }

        // DELETE: odata/RfidEventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            RfidEventLog rfidEventLog = await db.RfidEventLogs.FindAsync(key);
            if (rfidEventLog == null)
            {
                return NotFound();
            }

            db.RfidEventLogs.Remove(rfidEventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/RfidEventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.RfidEventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RfidEventLogExists(int key)
        {
            return db.RfidEventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
