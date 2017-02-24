using System;
using System.Linq;
using Keebee.AAT.ApiClient;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class ApiClientTests
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
            var media = client.GetForResident(residentId);

            // Assert
            Assert.IsNotNull(media);
            Assert.AreEqual(media.Resident.Id, residentId);
        }

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
            var streamId = new Guid("0d7434bc-cc81-e611-8aa6-90e6bac7161a");

            // Act
            var streamIds = client.GetForStreamId(streamId);

            // Assert
            Assert.IsNotNull(streamIds);
            Assert.IsTrue(streamIds.Any());
        }

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

        [TestMethod]
        public void GetExportFiles()
        {
            var client = new MediaFilesClient();
            var mediaPath = new MediaSourcePath();

            // Act
            var mediaPaths = client.GetForPath(mediaPath.ExportEventLogRoot);

            // Assert
            Assert.IsNotNull(mediaPaths);
            Assert.IsTrue(mediaPaths.Any());
        }
    }
}
