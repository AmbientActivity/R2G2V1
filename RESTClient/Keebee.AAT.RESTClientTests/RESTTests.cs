using System;
using System.Linq;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.RESTClientTests
{
    [TestClass]
    public class RESTTests
    {
        readonly OperationsClient _client = new OperationsClient();

        [TestMethod]
        public void GetLinkedResidentMedia()
        {
            // Act
            var residentMedia = _client.GetResidentMediaLinked();

            // Assert
            Assert.IsNotNull(residentMedia);
        }

        [TestMethod]
        public void GetResidentWithMedia()
        {
            // Arrange
            const int residentId = 1;

            // Act
            var resident = _client.GetResidentWithMedia(residentId);

            // Assert
            Assert.IsNotNull(resident);
            Assert.IsTrue(resident.MediaFiles.Any());
            Assert.AreEqual(resident.Id, residentId);
        }

        [TestMethod]
        public void GetResidentMedia()
        {
            // Arrange
            const int residentId = 1;

            // Act
            var media = _client.GetResidentMediaFilesForResident(residentId);

            // Assert
            Assert.IsNotNull(media);
            Assert.AreEqual(media.Resident.Id, residentId);
        }

        [TestMethod]
        public void GetPublicMedia()
        {
            // Act
            var media = _client.GetPublicMediaFiles();

            // Assert
            Assert.IsNotNull(media);
            Assert.IsTrue(media.MediaFiles.Any());
        }

        [TestMethod]
        public void GetPublicMediaFilesForStreamId()
        {
            // Arrange
            var streamId = new Guid("0d7434bc-cc81-e611-8aa6-90e6bac7161a");

            // Act
            var streamIds = _client.GetPublicMediaFilesForStreamId(streamId);

            // Assert
            Assert.IsNotNull(streamIds);
            Assert.IsTrue(streamIds.Any());
        }

        [TestMethod]
        public void GetMediaPathTypes()
        {
            // Act
            var mediaPaths = _client.GetMediaPathTypes();

            // Assert
            Assert.IsNotNull(mediaPaths);
            Assert.IsTrue(mediaPaths.Any());
        }

        [TestMethod]
        public void GetExportFiles()
        {
            MediaSourcePath mediaPath = new MediaSourcePath();

            // Act
            var mediaPaths = _client.GetMediaFilesForPath(mediaPath.ExportEventLogRoot);

            // Assert
            Assert.IsNotNull(mediaPaths);
            Assert.IsTrue(mediaPaths.Any());
        }
    }
}
