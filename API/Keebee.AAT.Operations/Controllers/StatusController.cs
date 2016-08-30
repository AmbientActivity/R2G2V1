using Keebee.AAT.Operations.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        // gets a count of ActivityTypes (for the KeepAlive routine in the StateMachineService)
        public async Task<int> Get()
        {
            return await Task.Run(() => _statusService.Get());
        }
    }
}
