using Keebee.AAT.DataAccess.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class PersonalPicturesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/PersonalPictures
        [EnableQuery]
        public IQueryable<PersonalPicture> GetPersonalPictures()
        {
            return db.PersonalPictures.OrderBy(o => o.Id);
        }

        // GET: odata/PersonalPictures(5)
        [EnableQuery]
        public SingleResult<PersonalPicture> GetPersonalPicture([FromODataUri] int key)
        {
            return SingleResult.Create(db.PersonalPictures.Where(picture => picture.Id == key));
        }

        // PUT: odata/PersonalPictures(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<PersonalPicture> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PersonalPicture picture = await db.PersonalPictures.FindAsync(key);
            if (picture == null)
            {
                return NotFound();
            }

            patch.Put(picture);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonalPictureExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(picture);
        }

        // POST: odata/PersonalPicture
        public async Task<IHttpActionResult> Post(PersonalPicture picture)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PersonalPictures.Add(picture);
            await db.SaveChangesAsync();

            return Created(picture);
        }

        // PATCH: odata/PersonalPictures(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<PersonalPicture> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PersonalPicture picture = await db.PersonalPictures.FindAsync(key);
            if (picture == null)
            {
                return NotFound();
            }

            patch.Patch(picture);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonalPictureExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(picture);
        }

        // DELETE: odata/PersonalPictures(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            PersonalPicture response = await db.PersonalPictures.FindAsync(key);
            if (response == null)
            {
                return NotFound();
            }

            db.PersonalPictures.Remove(response);
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

        private bool PersonalPictureExists(int key)
        {
            return db.PersonalPictures.Count(e => e.Id == key) > 0;
        }
    }
}
