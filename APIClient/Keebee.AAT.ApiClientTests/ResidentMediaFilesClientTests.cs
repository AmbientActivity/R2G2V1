using Keebee.AAT.ApiClient.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class ResidentMediaFilesClientTests
    {
        [TestMethod]
        public void GetLinkedResidentMediaList()
        {
            // Arrange
            var client = new ResidentMediaFilesClient();

            // Act
            var mediaList = client.GetLinked();

            // Assert
            Assert.IsNotNull(mediaList);
        }

        [TestMethod]
        public void GetResidentMediaList()
        {
            // Arrange
            var client = new ResidentMediaFilesClient();

            // Act
            var media = client.Get();

            // Assert
            Assert.IsNotNull(media);
        }

        [TestMethod]
        public void GetResidentMediaForResident()
        {
            // Arrange
            var client = new ResidentMediaFilesClient();
            const int residentId = 1;

            // Act
            var paths = client.GetForResident(residentId);

            // Assert
            Assert.IsNotNull(paths);
        }
    }
}
