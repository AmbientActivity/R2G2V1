using Keebee.AAT.ApiClient.Clients;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class ResidentsClientTests
    {
        [TestMethod]
        public void GetResidentWithMedia()
        {
            // Arrange
            var client = new ResidentsClient();
            const int residentId = 1;

            // Act
            var resident = client.GetWithMedia(residentId);

            // Assert
            Assert.IsNotNull(resident);
            Assert.IsTrue(resident.MediaFiles.Any());
            Assert.AreEqual(resident.Id, residentId);
        }
    }
}
