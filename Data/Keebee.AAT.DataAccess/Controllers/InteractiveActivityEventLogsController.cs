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
    public class InteractiveActivityEventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/InteractiveActivityEventLogs
        [EnableQuery]
        public IQueryable<InteractiveActivityEventLog> GetInteractiveActivityEventLogs()
        {
            return db.InteractiveActivityEventLogs;
        }

        // GET: odata/InteractiveActivityEventLogs(5)
        [EnableQuery]
        public SingleResult<InteractiveActivityEventLog> GetInteractiveActivityEventLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.InteractiveActivityEventLogs.Where(interactiveActivityEventLog => interactiveActivityEventLog.Id == key));
        }

        // PUT: odata/InteractiveActivityEventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<InteractiveActivityEventLog> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InteractiveActivityEventLog interactiveActivityEventLog = await db.InteractiveActivityEventLogs.FindAsync(key);
            if (interactiveActivityEventLog == null)
            {
                return NotFound();
            }

            patch.Put(interactiveActivityEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InteractiveActivityEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(interactiveActivityEventLog);
        }

        // POST: odata/InteractiveActivityEventLogs
        public async Task<IHttpActionResult> Post(InteractiveActivityEventLog interactiveActivityEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            interactiveActivityEventLog.DateEntry = DateTime.Now;
            db.InteractiveActivityEventLogs.Add(interactiveActivityEventLog);
            await db.SaveChangesAsync();

            return Created(interactiveActivityEventLog);
        }

        // PATCH: odata/InteractiveActivityEventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<InteractiveActivityEventLog> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InteractiveActivityEventLog interactiveActivityEventLog = await db.InteractiveActivityEventLogs.FindAsync(key);
            if (interactiveActivityEventLog == null)
            {
                return NotFound();
            }

            patch.Patch(interactiveActivityEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InteractiveActivityEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(interactiveActivityEventLog);
        }

        // DELETE: odata/InteractiveActivityEventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            InteractiveActivityEventLog interactiveActivityEventLog = await db.InteractiveActivityEventLogs.FindAsync(key);
            if (interactiveActivityEventLog == null)
            {
                return NotFound();
            }

            db.InteractiveActivityEventLogs.Remove(interactiveActivityEventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/InteractiveActivityEventLogs(5)/InteractiveActivityType
        [EnableQuery]
        public SingleResult<InteractiveActivityType> GetInteractiveActivityType([FromODataUri] int key)
        {
            return SingleResult.Create(db.InteractiveActivityEventLogs.Where(m => m.Id == key).Select(m => m.InteractiveActivityType));
        }

        // GET: odata/InteractiveActivityEventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.InteractiveActivityEventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InteractiveActivityEventLogExists(int key)
        {
            return db.InteractiveActivityEventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
