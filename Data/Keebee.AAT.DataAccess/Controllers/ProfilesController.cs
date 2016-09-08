using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ProfilesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/Profiles
        [EnableQuery(MaxExpansionDepth = 4)]
        public IQueryable<Profile> Get()
        {
            return db.Profiles.OrderBy(o => o.Id);
        }

        // GET: odata/Profiles(5)
        [EnableQuery(MaxExpansionDepth = 3)]
        public SingleResult<Profile> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.Profiles.Where(profile => profile.Id == key));
        }

        // PUT: odata/Profiles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Profile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Profile profile = await db.Profiles.FindAsync(key);
            if (profile == null)
            {
                return NotFound();
            }

            patch.Put(profile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(profile);
        }

        // POST: odata/Profiles
        public async Task<IHttpActionResult> Post(Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Profiles.Add(profile);
            await db.SaveChangesAsync();

            return Created(profile);
        }

        // PATCH: odata/Profiles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Profile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Profile profile = await db.Profiles.FindAsync(key);
            if (profile == null)
            {
                return NotFound();
            }

            patch.Patch(profile);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(profile);
        }

        // DELETE: odata/Profiles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Profile profile = await db.Profiles.FindAsync(key);
            if (profile == null)
            {
                return NotFound();
            }

            db.Profiles.Remove(profile);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Profiles(5)/ProfileDetails
        //[EnableQuery]
        //public IQueryable<ProfileDetail> GetProfileDetails([FromODataUri] int key)
        //{
        //    return db.Profiles.Where(m => m.Id == key).SelectMany(m => m.ProfileDetails);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileExists(int key)
        {
            return db.Profiles.Count(e => e.Id == key) > 0;
        }
    }
}
