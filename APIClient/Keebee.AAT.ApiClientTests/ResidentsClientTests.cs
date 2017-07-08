using Keebee.AAT.ApiClient.Clients;
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
            Assert.AreEqual(resident.Id, residentId);
        }

        [TestMethod]
        public void TryGetNonExistentResident()
        {
            // Arrange
            var client = new ResidentsClient();
            const int residentId = -1;

            // Act
            var resident = client.Get(residentId);

            // Assert
            Assert.IsNotNull(resident);
            Assert.IsNull(resident.FirstName);
        }

        [TestMethod]
        public void GetResidentExists()
        {
            // Arrange
            var client = new ResidentsClient();
            const int residentId = 12;

            // Act
            var exists = client.Exists(residentId);

            // Assert
            Assert.IsFalse(exists);
        }
    }
}
