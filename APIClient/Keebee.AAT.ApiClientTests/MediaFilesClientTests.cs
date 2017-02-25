using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.Shared;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class MediaFilesClientTests
    {
        [TestMethod]
        public void GetSharedLibraryFiles()
        {
            var client = new MediaFilesClient();
            var mediaPath = new MediaSourcePath();

            // Act
            var mediaPaths = client.GetForPath(mediaPath.SharedLibrary);

            // Assert
            Assert.IsNotNull(mediaPaths);
            Assert.IsTrue(mediaPaths.Any());
        }
    }
}
