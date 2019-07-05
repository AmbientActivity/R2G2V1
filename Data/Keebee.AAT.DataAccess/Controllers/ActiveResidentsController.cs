using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ActiveResidentsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ActiveResidents
        [EnableQuery]
        public IQueryable<ActiveResident> Get()
        {
            return db.ActiveResidents;
        }

        // GET: odata/ActiveResidents(1)
        [EnableQuery]
        public SingleResult<ActiveResident> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.ActiveResidents.Where(activetResident => activetResident.Id == key));
        }

        // PUT: odata/ActiveResidents(1)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ActiveResident> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActiveResident activetResident = await db.ActiveResidents.FindAsync(key);
            if (activetResident == null)
            {
                return NotFound();
            }

            patch.Put(activetResident);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivetResidentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activetResident);
        }

        // POST: odata/ActiveResidents
        public async Task<IHttpActionResult> Post(ActiveResident activetResident)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ActiveResidents.Add(activetResident);
            await db.SaveChangesAsync();

            return Created(activetResident);
        }

        // PATCH: odata/ActiveResidents(1)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ActiveResident> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ActiveResident activetResident = await db.ActiveResidents.FindAsync(key);
            if (activetResident == null)
            {
                return NotFound();
            }

            patch.Patch(activetResident);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivetResidentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activetResident);
        }

        // DELETE: odata/ActiveResidents(1)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ActiveResident activetResident = await db.ActiveResidents.FindAsync(key);
            if (activetResident == null)
            {
                return NotFound();
            }

            db.ActiveResidents.Remove(activetResident);
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

        private bool ActivetResidentExists(int key)
        {
            return db.ActiveResidents.Count(e => e.Id == key) > 0;
        }
    }
}
