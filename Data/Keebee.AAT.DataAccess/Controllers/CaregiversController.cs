using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class CaregiversController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/Caregivers
        [EnableQuery]
        public IQueryable<Caregiver> GetCaregivers()
        {
            return db.Caregivers;
        }

        // GET: odata/Caregivers(5)
        [EnableQuery]
        public SingleResult<Caregiver> GetCaregiver([FromODataUri] int key)
        {
            return SingleResult.Create(db.Caregivers.Where(caregiver => caregiver.Id == key));
        }

        // PUT: odata/Caregivers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Caregiver> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Caregiver caregiver = await db.Caregivers.FindAsync(key);
            if (caregiver == null)
            {
                return NotFound();
            }

            patch.Put(caregiver);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaregiverExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(caregiver);
        }

        // POST: odata/Caregivers
        public async Task<IHttpActionResult> Post(Caregiver caregiver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Caregivers.Add(caregiver);
            await db.SaveChangesAsync();

            return Created(caregiver);
        }

        // PATCH: odata/Caregivers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Caregiver> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Caregiver caregiver = await db.Caregivers.FindAsync(key);
            if (caregiver == null)
            {
                return NotFound();
            }

            patch.Patch(caregiver);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaregiverExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(caregiver);
        }

        // DELETE: odata/Caregivers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Caregiver caregiver = await db.Caregivers.FindAsync(key);
            if (caregiver == null)
            {
                return NotFound();
            }

            db.Caregivers.Remove(caregiver);
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

        private bool CaregiverExists(int key)
        {
            return db.Caregivers.Count(e => e.Id == key) > 0;
        }
    }
}
