using Keebee.AAT.ApiClient.Clients;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class PublicMediaFilesClientTests
    {
        [TestMethod]
        public void GetPublicMedia()
        {
            // Act
            var client = new PublicMediaFilesClient();
            var media = client.Get();

            // Assert
            Assert.IsNotNull(media);
            Assert.IsTrue(media.MediaFiles.Any());
        }

        [TestMethod]
        public void GetLinkedPublicMedia()
        {
            // Act
            var client = new PublicMediaFilesClient();
            var media = client.GetLinked();

            // Assert
            Assert.IsNotNull(media);
            Assert.IsTrue(media.MediaFiles.Any());
        }

        [TestMethod]
        public void GetPublicMediaFilesForStreamId()
        {
            // Arrange
            var client = new PublicMediaFilesClient();
            var streamId = new Guid("9e539859-7cf4-e611-9cb8-98eecb38d473");

            // Act
            var streamIds = client.GetForStreamId(streamId);

            // Assert
            Assert.IsNotNull(streamIds);
            Assert.IsTrue(streamIds.Any());
        }
    }
}
