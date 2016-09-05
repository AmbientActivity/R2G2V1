using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class GameTypesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/GameTypes
        [EnableQuery]
        public IQueryable<GameType> GetGameTypes()
        {
            return db.GameTypes;
        }

        // GET: odata/GameTypes(5)
        [EnableQuery]
        public SingleResult<GameType> GetGameType([FromODataUri] int key)
        {
            return SingleResult.Create(db.GameTypes.Where(gameType => gameType.Id == key));
        }

        // PUT: odata/GameTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<GameType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GameType gameType = await db.GameTypes.FindAsync(key);
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
                if (!GameTypeExists(key))
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

        // POST: odata/GameTypes
        public async Task<IHttpActionResult> Post(GameType gameType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GameTypes.Add(gameType);
            await db.SaveChangesAsync();

            return Created(gameType);
        }

        // PATCH: odata/GameTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<GameType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GameType gameType = await db.GameTypes.FindAsync(key);
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
                if (!GameTypeExists(key))
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

        // DELETE: odata/GameTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            GameType gameType = await db.GameTypes.FindAsync(key);
            if (gameType == null)
            {
                return NotFound();
            }

            db.GameTypes.Remove(gameType);
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

        private bool GameTypeExists(int key)
        {
            return db.GameTypes.Count(e => e.Id == key) > 0;
        }
    }
}
