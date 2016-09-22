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
        public void GetAmbientMedia()
        {
            //// Act
            //var media = _client.GetAmbientMediaFiles();

            //// Assert
            //Assert.IsNotNull(media);
            //Assert.IsTrue(media.MediaFiles.Any());
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
