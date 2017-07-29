using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/AmbientInvitations")]
    public class AmbientInvitationsController : ApiController
    {
        private readonly IAmbientInvitationService _ambientInvitationService;

        public AmbientInvitationsController(IAmbientInvitationService ambientInvitationService)
        {
            _ambientInvitationService = ambientInvitationService;
        }

        // GET: api/AmbientInvitations
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<AmbientInvitation> ambientInvitations = new Collection<AmbientInvitation>();

            await Task.Run(() =>
            {
                ambientInvitations = _ambientInvitationService.Get();
            });

            if (ambientInvitations == null) return new DynamicJsonArray(new object[0]);

            var jArray = ambientInvitations
                .Select(x => new
                {
                    x.Id,
                    x.Message, 
                    ResponseType = new
                    {
                        Id = x.ResponseType?.Id ?? 0,
                        x.ResponseType?.Description,
                        ResponseTypeCategory = new
                        {
                            Id = x.ResponseType?.ResponseTypeCategory.Id ?? 0,
                            x.ResponseType?.ResponseTypeCategory.Description
                        },
                        InteractiveActivityType = new
                        {
                            Id = x.ResponseType?.InteractiveActivityType?.Id ?? 0,
                            x.ResponseType?.InteractiveActivityType?.Description,
                            x.ResponseType?.InteractiveActivityType?.SwfFile
                        }
                    }
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/AmbientInvitations/1
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var ambientInvitation = new AmbientInvitation();

            await Task.Run(() =>
            {
                ambientInvitation = _ambientInvitationService.Get(id);
            });

            if (ambientInvitation == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = ambientInvitation.Id;
            exObj.Resident = new
            {
                ambientInvitation.Id,
                ambientInvitation.Message,
                ResponseType = new
                {
                    Id = ambientInvitation.ResponseType?.Id ?? 0,
                    ResponseTypeCategoryType = new
                    {
                        Id = ambientInvitation.ResponseType?.ResponseTypeCategory.Id ?? 0,
                        ambientInvitation.ResponseType?.ResponseTypeCategory.Description
                    },
                    InteractiveActivityType = new
                    {
                        Id = ambientInvitation.ResponseType?.InteractiveActivityType?.Id ?? 0,
                        ambientInvitation.ResponseType?.InteractiveActivityType?.Description,
                        ambientInvitation.ResponseType?.InteractiveActivityType?.SwfFile
                    }
                }
            };
            return new DynamicJsonObject(exObj);
        }

        // POST: api/AmbientInvitations
        [HttpPost]
        public int Post([FromBody]AmbientInvitation ambientInvitation)
        {
            return _ambientInvitationService.Post(ambientInvitation);
        }

        // PATCH: api/AmbientInvitations/1
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]AmbientInvitation ambientInvitation)
        {
            _ambientInvitationService.Patch(id, ambientInvitation);
        }

        // DELETE: api/AmbientInvitations/1
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _ambientInvitationService.Delete(id);
        }
    }
}
