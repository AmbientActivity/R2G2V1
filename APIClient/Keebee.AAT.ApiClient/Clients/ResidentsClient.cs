using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IResidentsClient
    {
        IEnumerable<Resident> Get();
        Resident Get(int residentId);
        Resident GetWithMedia(int residentId);
        Resident GetByNameGender(string firstName, string lastName, string gender);
        bool Exists(int residentId);
        void Patch(int residentId, ResidentEdit resident);
        int Post(ResidentEdit resident);
    }

    public class ResidentsClient : IResidentsClient
    {
        private readonly ClientBase _clientBase;

        public ResidentsClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<Resident> Get()
        {
            var data = _clientBase.Get("residents");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residents = serializer.Deserialize<ResidentList>(data).Residents.ToList();

            return residents;
        }

        public Resident Get(int residentId)
        {
            var data = _clientBase.Get($"residents/{residentId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public Resident GetByNameGender(string firstName, string lastName, string gender)
        {
            var data = _clientBase.Get($"residents?firstName={firstName}&lastName={lastName}&gender={gender}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public Resident GetWithMedia(int residentId)
        {
            var data = _clientBase.Get($"residents/{residentId}/media");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public bool Exists(int residentId)
        {
            return _clientBase.Exists($"residents/{residentId}");
        }

        public void Patch(int residentId, ResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            _clientBase.Patch($"residents/{residentId}", el);
        }

        public int Post(ResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            return _clientBase.Post("residents", el);
        }

        public string Delete(int residentId)
        {
            return _clientBase.Delete($"residents/{residentId}");
        }
    }
}
