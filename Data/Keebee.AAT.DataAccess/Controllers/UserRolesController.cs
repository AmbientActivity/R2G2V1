using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class UserRolesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/UserRoles
        [EnableQuery(MaxExpansionDepth = 3)]
        public IQueryable<UserRole> Get()
        {
            return db.UserRoles.OrderBy(o => o.Id);
        }

        // GET: odata/UserRoles(5)
        [EnableQuery(MaxExpansionDepth = 3)]
        public SingleResult<UserRole> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.UserRoles.Where(userRole => userRole.Id == key));
        }

        // PUT: odata/UserRoles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<UserRole> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserRole userRole = await db.UserRoles.FindAsync(key);
            if (userRole == null)
            {
                return NotFound();
            }

            patch.Put(userRole);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userRole);
        }

        // POST: odata/UserRoles
        public async Task<IHttpActionResult> Post(UserRole userRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserRoles.Add(userRole);
            await db.SaveChangesAsync();

            return Created(userRole);
        }

        // PATCH: odata/UserRoles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<UserRole> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserRole userRole = await db.UserRoles.FindAsync(key);
            if (userRole == null)
            {
                return NotFound();
            }

            patch.Patch(userRole);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userRole);
        }

        // DELETE: odata/UserRoles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            UserRole userRole = await db.UserRoles.FindAsync(key);
            if (userRole == null)
            {
                return NotFound();
            }

            db.UserRoles.Remove(userRole);
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

        private bool UserRoleExists(int key)
        {
            return db.UserRoles.Count(e => e.Id == key) > 0;
        }
    }
}
