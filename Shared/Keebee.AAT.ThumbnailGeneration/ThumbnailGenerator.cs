using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using System;
using System.IO;

namespace Keebee.AAT.ThumbnailGeneration
{
    public class ThumbnailGenerator
    {
        private readonly IMediaFileStreamsClient _mediaFileStreamsClient;
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        public ThumbnailGenerator()
        {
            _mediaFileStreamsClient = new MediaFileStreamsClient();
            _mediaFilesClient = new MediaFilesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        public string Generate(Guid streamId)
        {
            string errorMessage = null;

            try
            {
                var image = Get(streamId, out errorMessage);
                if (errorMessage != null) throw new Exception(errorMessage);

                errorMessage = Save(streamId, image, true);
                if (errorMessage != null) throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public byte[] GetAndSave(Guid streamId, bool overwrite, out string errorMessage)
        {
            byte[] image = null;

            try
            {
                image = Get(streamId, out errorMessage);
                if (errorMessage != null) throw new Exception(errorMessage);

                errorMessage = Save(streamId, image, overwrite);
                if (errorMessage != null) throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return image;
        }

        public byte[] Get(Guid streamId, out string errorMessage)
        {
            byte[] image = null;
            errorMessage = null;

            try
            {
                var mediaFile = _mediaFilesClient.Get(streamId);
                var categoryId = GetCategoryId(mediaFile.FileType);

                switch (categoryId)
                {
                    case MediaPathTypeCategoryId.Image:
                        var fileStream = _mediaFileStreamsClient.Get(mediaFile.StreamId);
                        image = Helpers.GetImageThumbnail(fileStream, out errorMessage);
                        if (errorMessage != null) throw new Exception(errorMessage);
                        break;
                    case MediaPathTypeCategoryId.Video:
                        image = Helpers.GetVideoThumbnail(Path.Combine(mediaFile.Path, mediaFile.Filename), out errorMessage);
                        if (errorMessage != null) throw new Exception(errorMessage);
                        break;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return image;
        }

        public string Save(Guid streamId, byte[] image, bool overwrite)
        {
            string errorMessage = null;

            try
            {
                var exists = false;

                if (image != null)
                {
                    if (overwrite) exists = ThumbnailExists(streamId);

                    if (exists)
                    {
                        errorMessage = _thumbnailsClient.Patch(streamId, new Thumbnail {Image = image});
                        if (errorMessage != null) throw new Exception(errorMessage);
                    }
                    else
                    {
                        errorMessage = _thumbnailsClient.Post(new Thumbnail {StreamId = streamId, Image = image});
                        if (errorMessage != null) throw new Exception(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public string Delete(Guid streamId)
        {
            string errorMessage = null;

            try
            {
                if (ThumbnailExists(streamId))
                {
                    errorMessage = _thumbnailsClient.Delete(streamId);
                    if (errorMessage != null) throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public bool ThumbnailExists(Guid streamId)
        {
            var thumb = _thumbnailsClient.Get(streamId);
            return (thumb.Image != null);
        }

        private static int GetCategoryId(string fileType)
        {
            switch (fileType.ToLower())
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "bmp":
                    return MediaPathTypeCategoryId.Image;
                case "mp3":
                    return MediaPathTypeCategoryId.Audio;
                case "mp4":
                    return MediaPathTypeCategoryId.Video;
                default:
                    return -1;
            }
        }
    }
}
