using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class AmbientInvitationsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/AmbientInvitations
        [EnableQuery]
        public IQueryable<AmbientInvitation> Get()
        {
            return db.AmbientInvitations;
        }

        // GET: odata/AmbientInvitations(1)
        [EnableQuery]
        public SingleResult<AmbientInvitation> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.AmbientInvitations.Where(ambientInvitation => ambientInvitation.Id == key));
        }

        // PUT: odata/AmbientInvitations(1)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<AmbientInvitation> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AmbientInvitation ambientInvitation = await db.AmbientInvitations.FindAsync(key);
            if (ambientInvitation == null)
            {
                return NotFound();
            }

            patch.Put(ambientInvitation);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AmbientInvitationExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ambientInvitation);
        }

        // POST: odata/AmbientInvitations
        public async Task<IHttpActionResult> Post(AmbientInvitation ambientInvitation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AmbientInvitations.Add(ambientInvitation);
            await db.SaveChangesAsync();

            return Created(ambientInvitation);
        }

        // PATCH: odata/AmbientInvitations(1)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<AmbientInvitation> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AmbientInvitation ambientInvitation = await db.AmbientInvitations.FindAsync(key);
            if (ambientInvitation == null)
            {
                return NotFound();
            }

            patch.Patch(ambientInvitation);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AmbientInvitationExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ambientInvitation);
        }

        // DELETE: odata/AmbientInvitations(1)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            AmbientInvitation ambientInvitation = await db.AmbientInvitations.FindAsync(key);
            if (ambientInvitation == null)
            {
                return NotFound();
            }

            db.AmbientInvitations.Remove(ambientInvitation);
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

        private bool AmbientInvitationExists(int key)
        {
            return db.AmbientInvitations.Count(e => e.Id == key) > 0;
        }
    }
}
