using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ProfileDetailsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ProfileDetails
        [EnableQuery]
        public IQueryable<ProfileDetail> Get()
        {
            return db.ProfileDetails;
        }

        // GET: odata/ProfileDetails(5)
        [EnableQuery]
        public SingleResult<ProfileDetail> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.ProfileDetails.Where(profileDetail => profileDetail.Id == key));
        }

        // PUT: odata/ProfileDetails(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ProfileDetail> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProfileDetail profileDetail = await db.ProfileDetails.FindAsync(key);
            if (profileDetail == null)
            {
                return NotFound();
            }

            patch.Put(profileDetail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileDetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(profileDetail);
        }

        // POST: odata/ProfileDetails
        public async Task<IHttpActionResult> Post(ProfileDetail profileDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProfileDetails.Add(profileDetail);
            await db.SaveChangesAsync();

            return Created(profileDetail);
        }

        // PATCH: odata/ProfileDetails(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ProfileDetail> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProfileDetail profileDetail = await db.ProfileDetails.FindAsync(key);
            if (profileDetail == null)
            {
                return NotFound();
            }

            patch.Patch(profileDetail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileDetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(profileDetail);
        }

        // DELETE: odata/ProfileDetails(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ProfileDetail profileDetail = await db.ProfileDetails.FindAsync(key);
            if (profileDetail == null)
            {
                return NotFound();
            }

            db.ProfileDetails.Remove(profileDetail);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ProfileDetails(5)/Activity
        [EnableQuery]
        public SingleResult<ActivityType> GetActivityType([FromODataUri] int key)
        {
            return SingleResult.Create(db.ProfileDetails.Where(m => m.Id == key).Select(m => m.ActivityType));
        }

        // GET: odata/ProfileDetails(5)/Profile
        [EnableQuery]
        public SingleResult<Profile> GetProfile([FromODataUri] int key)
        {
            return SingleResult.Create(db.ProfileDetails.Where(m => m.Id == key).Select(m => m.Profile));
        }

        // GET: odata/ProfileDetails(5)/Responses
        [EnableQuery]
        public IQueryable<Response> GetResponses([FromODataUri] int key)
        {
            return db.ProfileDetails.Where(m => m.Id == key).SelectMany(m => m.Responses);
        }

        // GET: odata/ProfileDetails(5)/ResponseType
        [EnableQuery]
        public SingleResult<ResponseType> GetResponseType([FromODataUri] int key)
        {
            return SingleResult.Create(db.ProfileDetails.Where(m => m.Id == key).Select(m => m.ResponseType));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileDetailExists(int key)
        {
            return db.ProfileDetails.Count(e => e.Id == key) > 0;
        }
    }
}
