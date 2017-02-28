using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class ActiveResidentClientTests
    {
        [TestMethod]
        public void GetActiveResident()
        {
            // Arrange
            var client = new ActiveResidentClient();

            // Act
            var resident = client.Get();

            // Assert
            Assert.IsNotNull(resident);
        }

        [TestMethod]
        public void PostActiveResident()
        {
            // Arrange
            var client = new ActiveResidentClient();
            var activeResident = new ActiveResidentEdit
            {
                Id = 1,
                ResidentId = 2
            };

            // Act
            client.Patch(activeResident);

            // Assert
            //Assert.IsNotNull(resident);
        }
    }
}