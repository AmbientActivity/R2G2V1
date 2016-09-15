using System;
using System.Linq;
using Keebee.AAT.Operations.Service.KeebeeAAT;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IStatusService
    {
        int Get();
    }

    public class StatusService : IStatusService
    {
        public int Get()
        {
            var container = new Container(new Uri(ODataHost.Url));
            // keep the state machine service responsive for when a new rfid gets read
            return container.Configs.Count();
        }
    }
}
