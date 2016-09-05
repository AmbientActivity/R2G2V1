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
    public class GameEventLogsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/GameEventLogs
        [EnableQuery]
        public IQueryable<GameEventLog> GetGameEventLogs()
        {
            return db.GameEventLogs;
        }

        // GET: odata/GameEventLogs(5)
        [EnableQuery]
        public SingleResult<GameEventLog> GetGameEventLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.GameEventLogs.Where(gameEventLog => gameEventLog.Id == key));
        }

        // PUT: odata/GameEventLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<GameEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GameEventLog gameEventLog = await db.GameEventLogs.FindAsync(key);
            if (gameEventLog == null)
            {
                return NotFound();
            }

            patch.Put(gameEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gameEventLog);
        }

        // POST: odata/GameEventLogs
        public async Task<IHttpActionResult> Post(GameEventLog gameEventLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            gameEventLog.DateEntry = DateTime.Now;
            db.GameEventLogs.Add(gameEventLog);
            await db.SaveChangesAsync();

            return Created(gameEventLog);
        }

        // PATCH: odata/GameEventLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<GameEventLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GameEventLog gameEventLog = await db.GameEventLogs.FindAsync(key);
            if (gameEventLog == null)
            {
                return NotFound();
            }

            patch.Patch(gameEventLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameEventLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gameEventLog);
        }

        // DELETE: odata/GameEventLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            GameEventLog gameEventLog = await db.GameEventLogs.FindAsync(key);
            if (gameEventLog == null)
            {
                return NotFound();
            }

            db.GameEventLogs.Remove(gameEventLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/GameEventLogs(5)/GameType
        [EnableQuery]
        public SingleResult<GameType> GetGameType([FromODataUri] int key)
        {
            return SingleResult.Create(db.GameEventLogs.Where(m => m.Id == key).Select(m => m.GameType));
        }

        // GET: odata/GameEventLogs(5)/Resident
        [EnableQuery]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.GameEventLogs.Where(m => m.Id == key).Select(m => m.Resident));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameEventLogExists(int key)
        {
            return db.GameEventLogs.Count(e => e.Id == key) > 0;
        }
    }
}
