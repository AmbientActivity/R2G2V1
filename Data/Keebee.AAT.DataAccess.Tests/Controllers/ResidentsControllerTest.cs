using Keebee.AAT.DataAccess.Tests.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.DataAccess.Tests.KeebeeAAT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Keebee.AAT.DataAccess.Tests.Controllers
{
    [TestClass]
    public class ResidentTests
    {
        private const string OdataUri = "http://localhost/Keebee.AAT.DataAccess/odata/";

        // ---- Container ----

        readonly Container _container = new Container(new Uri(OdataUri));

        [TestMethod]
        public void OData_Container_GeResident()
        {
            // Arrange
            const int residentId = 1;
            const int expectedProfileId = 2;

            // Act
            var profile = _container.Residents.ByKey(residentId)
                .Profile
                //.Expand("ProfileDetails($expand=Activity($expand=PhidgetType),Response($expand=ResponseType,Responses))")
                .GetValue();

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(profile.Id, expectedProfileId);
        }

        [TestMethod]
        public void OData_Container_GetAllResidents()
        {
            // Arrange
            const int expectedFirstResidentId = 1;

            // Act
            var residents = _container.Residents
                .ToArray()
                .OrderBy(x => x.Id);

            // Assert
            Assert.IsNotNull(residents);
            Assert.AreEqual(residents.First().Id, expectedFirstResidentId);
        }

        // ---- ODataClient ----

        private readonly ODataClient _client = new ODataClient(OdataUri);

        [TestMethod]
        public async Task OData_Client_GetResident()
        {
            // Arrange
            const int residentId = 1;

            // Act
            var result = await _client.For<Resident>()
                .Key(residentId)
                .NavigateTo(x => x.Profile)
                .FindEntriesAsync();

            var resident = result.Single();


            // Assert
            Assert.IsNotNull(resident);
            Assert.AreEqual(resident.Id, residentId);
        }

        [TestMethod]
        public async Task OData_Client_GetResponse()
        {
            // Arrange
            const int key = 1;   // Active Resident Id
            const int phidgetTypeId = 1;

            // Act
            var result = await _client.For<Resident>()
                .Key(key)
                .Function("Keebee.GetResponse")
                .Set(new { phidgetTypeId })
                .Expand("ResponseType")
                .ExecuteAsSingleAsync();

            var resident = result;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resident.Id, key);
        }

        // --- HttpRequest ---

        [TestMethod]
        public void OData_HttpRequest_GetResident()
        {
            // Arrange
            const int residentId = 1;

            var url = string.Format("{0}/Residents({1})", OdataUri, residentId);
            var request = (HttpWebRequest)WebRequest.Create(url);

            Resident resident;

            // Act
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var data = new StreamReader(stream);
                    resident = JsonConvert.DeserializeObject<Resident>(data.ReadToEnd());
                }
            }

            // Assert
            Assert.IsNotNull(resident);
            Assert.AreEqual(resident.Id, residentId);
        }

    }
}
