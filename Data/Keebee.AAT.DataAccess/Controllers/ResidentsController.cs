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
    public class ResidentsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/Residents
        [EnableQuery(MaxExpansionDepth = 4)]
        public IQueryable<Resident> GetResidents()
        {
            return db.Residents.OrderBy(o => o.Id);
        }

        // GET: odata/Residents(5)
        [EnableQuery(MaxExpansionDepth = 4)]
        public SingleResult<Resident> GetResident([FromODataUri] int key)
        {
            return SingleResult.Create(db.Residents.Where(resident => resident.Id == key));
        }

        // PUT: odata/Residents(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Resident> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Resident resident = await db.Residents.FindAsync(key);
            if (resident == null)
            {
                return NotFound();
            }

            patch.Put(resident);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResidentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(resident);
        }

        // POST: odata/Residents
        public async Task<IHttpActionResult> Post(Resident resident)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Residents.Add(resident);
            await db.SaveChangesAsync();

            return Created(resident);
        }

        // PATCH: odata/Residents(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Resident> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Resident resident = await db.Residents.FindAsync(key);
            if (resident == null)
            {
                return NotFound();
            }

            resident.DateUpdated = DateTime.Now;
            patch.Patch(resident);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResidentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(resident);
        }

        // DELETE: odata/Residents(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Resident resident = await db.Residents.FindAsync(key);
            if (resident == null)
            {
                return NotFound();
            }

            db.Residents.Remove(resident);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Residents(5)/Profile
        [EnableQuery(MaxExpansionDepth = 4)]
        public SingleResult<Profile> GetProfile([FromODataUri] int key)
        {
            return SingleResult.Create(db.Residents.Where(m => m.Id == key).Select(m => m.Profile));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ResidentExists(int key)
        {
            return db.Residents.Count(e => e.Id == key) > 0;
        }
    }
}
