using Keebee.AAT.ApiClient.Clients;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class MediaPathTypesClientTests
    {
        [TestMethod]
        public void GetMediaPathTypes()
        {
            // Act
            var client = new MediaPathTypesClient();
            var mediaPaths = client.Get();

            // Assert
            Assert.IsNotNull(mediaPaths);
            Assert.IsTrue(mediaPaths.Any());
        }
    }
}
