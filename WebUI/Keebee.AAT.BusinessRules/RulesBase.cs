using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.Shared;
using Keebee.AAT.ThumbnailGeneration;
using System;
using System.Drawing;
using System.IO;

namespace Keebee.AAT.BusinessRules
{
    public class RulesBase
    {
        public SystemEventLogger EventLogger { get; set; }

        public string DeleteFile(string filePath)
        {
            var fileManager = new FileManager{ EventLogger = EventLogger };
            return fileManager.DeleteFile(filePath);
        }

        public string CreateFolders(int profileId)
        {
            var fileManager = new FileManager { EventLogger = EventLogger };
            return fileManager.CreateFolders(profileId);
        }

        public string DeleteFolders(int profileId)
        {
            var fileManager = new FileManager { EventLogger = EventLogger };
            return fileManager.DeleteFolders(profileId);
        }

        public string SaveUploadedFile(string filename, string path, Stream filestream, string mediaPathTypeCategory)
        {
            string errMsg;

            try
            {
                var filePath = Path.Combine(path, filename);

                // delete it if it already exists
                errMsg = DeleteFile(filePath);
                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                // if it's an image, orient and resize
                if (mediaPathTypeCategory == MediaPathTypeCategoryDescription.Image)
                {
                    var image = Image.FromStream(filestream);
                    var orientedImage = image.Orient();
                    var scaled = orientedImage.Scale();
                    scaled.Save(filePath);
                }
                else
                {
                    using (var fileStream = File.Create(filePath))
                    {
                        filestream.Seek(0, SeekOrigin.Begin);
                        filestream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        public Guid GetStreamId(string path, string filename)
        {
            var fileManager = new FileManager { EventLogger = EventLogger };
            return fileManager.GetStreamId(path, filename);
        }

        public MediaFile GetMediaFile(string path, string filename)
        {
            var fileManager = new FileManager { EventLogger = EventLogger };
            return fileManager.GetMediaFile(path, filename);
        }

        public MediaFileModel GetMediaFileModel(
            MediaFile mediaFile,
            int id, 
            MediaPathType mediaPathType,
            DateTime dateAdded,
            bool isLinked,
            byte[] thumb)
        {
            return new MediaFileModel
            {
                Id = id,
                StreamId = mediaFile.StreamId,
                Filename = mediaFile.Filename.Replace($".{mediaFile.FileType}", string.Empty),
                FileType = mediaFile.FileType,
                FileSize = mediaFile.FileSize,
                IsLinked = isLinked,
                Path = mediaPathType.Path,
                MediaPathTypeId = mediaPathType.Id,
                DateAdded = dateAdded,
                Thumbnail = thumb != null
                                ? $"data:image/jpg;base64,{Convert.ToBase64String(thumb)}"
                                : string.Empty
            };
        }
    }
}
