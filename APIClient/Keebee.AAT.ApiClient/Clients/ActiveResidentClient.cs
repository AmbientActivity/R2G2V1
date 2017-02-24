using Keebee.AAT.ApiClient.Models;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IActiveResidentClient
    {
        ActiveResident Get();
        void Patch(ActiveResidentEdit resident);
    }

    public class ActiveResidentClient : IActiveResidentClient
    {
        private readonly ClientBase _clientBase;

        public ActiveResidentClient()
        {
            _clientBase = new ClientBase();
        }

        public ActiveResident Get()
        {
            var data = _clientBase.Get($"activeresidents/{1}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<ActiveResident>(data);

            return resident;
        }

        public void Patch(ActiveResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            _clientBase.Patch("activeresidents/1", el);
        }

    }
}
