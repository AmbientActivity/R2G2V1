using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IPhidgetStyleTypesClient
    {
        IEnumerable<PhidgetStyleType> Get();
    }

    public class PhidgetStyleTypesClient : IPhidgetStyleTypesClient
    {
        private readonly ClientBase _clientBase;

        public PhidgetStyleTypesClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<PhidgetStyleType> Get()
        {
            var data = _clientBase.Get("phidgetstyletypes");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var phidgetTypes = serializer.Deserialize<PhidgetStyleTypeList>(data).PhidgetStyleTypes;

            return phidgetTypes;
        }
    }
}
