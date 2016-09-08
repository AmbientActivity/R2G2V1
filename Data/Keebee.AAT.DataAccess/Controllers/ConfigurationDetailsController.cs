using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class ConfigurationDetailsController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/ConfigurationDetails
        public IQueryable<ConfigurationDetail> Get()
        {
            return db.ConfigurationDetails.OrderBy(o => o.Id);
        }

        // GET: odata/ConfigurationDetails(5)
        [EnableQuery(MaxExpansionDepth = 3)]
        public SingleResult<ConfigurationDetail> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.ConfigurationDetails.Where(configurationDetail => configurationDetail.Id == key));
        }

        // PUT: odata/ConfigurationDetails(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ConfigurationDetail> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ConfigurationDetail configurationDetail = await db.ConfigurationDetails.FindAsync(key);
            if (configurationDetail == null)
            {
                return NotFound();
            }

            patch.Put(configurationDetail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigurationDetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(configurationDetail);
        }

        // POST: odata/ConfigurationDetails
        public async Task<IHttpActionResult> Post(ConfigurationDetail configurationDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ConfigurationDetails.Add(configurationDetail);
            await db.SaveChangesAsync();

            return Created(configurationDetail);
        }

        // PATCH: odata/ConfigurationDetails(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ConfigurationDetail> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ConfigurationDetail configurationDetail = await db.ConfigurationDetails.FindAsync(key);
            if (configurationDetail == null)
            {
                return NotFound();
            }

            patch.Patch(configurationDetail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigurationDetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(configurationDetail);
        }

        // DELETE: odata/ConfigurationDetails(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ConfigurationDetail configurationDetail = await db.ConfigurationDetails.FindAsync(key);
            if (configurationDetail == null)
            {
                return NotFound();
            }

            db.ConfigurationDetails.Remove(configurationDetail);
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

        private bool ConfigurationDetailExists(int key)
        {
            return db.ConfigurationDetails.Count(e => e.Id == key) > 0;
        }
    }
}
