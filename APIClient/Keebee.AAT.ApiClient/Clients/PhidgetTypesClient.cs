using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IPhidgetTypesClient
    {
        IEnumerable<PhidgetType> Get();
    }

    public class PhidgetTypesClient : IPhidgetTypesClient
    {
        private readonly ClientBase _clientBase;

        public PhidgetTypesClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<PhidgetType> Get()
        {
            var data = _clientBase.Get("phidgettypes");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var phidgetTypes = serializer.Deserialize<PhidgetTypeList>(data).PhidgetTypes;

            return phidgetTypes;
        }
    }
}
