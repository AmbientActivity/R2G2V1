using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResponseTypesClient
    {
        IEnumerable<ResponseType> Get();
    }

    public class ResponseTypesClient : IResponseTypesClient
    {
        private readonly ClientBase _clientBase;

        public ResponseTypesClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<ResponseType> Get()
        {
            var data = _clientBase.Get("responsetypes");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var responseTypes = serializer.Deserialize<ResponseTypeList>(data).ResponseTypes;

            return responseTypes;
        }
    }
}
