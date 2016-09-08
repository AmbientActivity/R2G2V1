using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ConfigurationsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        [EnableQuery(MaxExpansionDepth = 4)]
        // GET: odata/Configurations
        public IQueryable<Configuration> Get()
        {
            return db.Configurations.OrderBy(o => o.Id);
        }

        // GET: odata/Configurations(5)
        [EnableQuery(MaxExpansionDepth = 4)]
        public SingleResult<Configuration> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.Configurations.Where(configuration => configuration.Id == key));
        }

        // PUT: odata/Configurations(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Configuration> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Configuration configuration = await db.Configurations.FindAsync(key);
            if (configuration == null)
            {
                return NotFound();
            }

            patch.Put(configuration);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigurationExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(configuration);
        }

        // POST: odata/Configurations
        public async Task<IHttpActionResult> Post(Configuration configuration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Configurations.Add(configuration);
            await db.SaveChangesAsync();

            return Created(configuration);
        }

        // PATCH: odata/Configurations(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Configuration> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Configuration configuration = await db.Configurations.FindAsync(key);
            if (configuration == null)
            {
                return NotFound();
            }

            patch.Patch(configuration);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigurationExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(configuration);
        }

        // DELETE: odata/Configurations(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Configuration configuration = await db.Configurations.FindAsync(key);
            if (configuration == null)
            {
                return NotFound();
            }

            db.Configurations.Remove(configuration);
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

        private bool ConfigurationExists(int key)
        {
            return db.Configurations.Count(e => e.Id == key) > 0;
        }
    }
}
