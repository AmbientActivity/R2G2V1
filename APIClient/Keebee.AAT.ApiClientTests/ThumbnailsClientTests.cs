using System;
using Keebee.AAT.ApiClient.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class ThumbnailsClientTests
    {
        [TestMethod]
        public void TryGetNonExistentThumbnail()
        {
            // Arrange
            var client = new ThumbnailsClient();
            var thumbnailId = new Guid("00000000-0000-0000-0000-000000000000");

            // Act
            var thumbnail = client.Get(thumbnailId);

            // Assert
            Assert.IsNotNull(thumbnail);
            Assert.IsNull(thumbnail.Image);
        }
    }
}
