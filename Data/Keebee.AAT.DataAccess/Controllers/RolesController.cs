using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class RolesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/Roles
        [EnableQuery]
        public IQueryable<Role> GetRoles()
        {
            return db.Roles.OrderBy(o => o.Id);
        }

        // GET: odata/Roles(5)
        [EnableQuery]
        public SingleResult<Role> GetRole([FromODataUri] int key)
        {
            return SingleResult.Create(db.Roles.Where(role => role.Id == key));
        }

        // PUT: odata/Roles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Role> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role role = await db.Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound();
            }

            patch.Put(role);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(role);
        }

        // POST: odata/Roles
        public async Task<IHttpActionResult> Post(Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Roles.Add(role);
            await db.SaveChangesAsync();

            return Created(role);
        }

        // PATCH: odata/Roles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Role> patch)
        {
            Validate(patch.GetInstance());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role role = await db.Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound();
            }

            patch.Patch(role);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(role);
        }

        // DELETE: odata/Roles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Role role = await db.Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound();
            }

            db.Roles.Remove(role);
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

        private bool RoleExists(int key)
        {
            return db.Roles.Count(e => e.Id == key) > 0;
        }
    }
}
