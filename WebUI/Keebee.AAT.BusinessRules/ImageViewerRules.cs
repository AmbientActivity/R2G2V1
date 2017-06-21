using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using System;
using System.Drawing;
using System.IO;

namespace Keebee.AAT.BusinessRules
{
    public class ImageViewerRules
    {
        public static class PreviewConstants
        {
            public const int MaxImagePreviewgWidth = 437;
            public const int MaxImagePreviewHeight = 333;
        }

        private struct ImageSize
        {
            public int Width;
            public int Height;
        }

        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IMediaFileStreamsClient _mediaFileStreamsClient;

        public ImageViewerRules()
        {
            _mediaFilesClient = new MediaFilesClient();
            _mediaFileStreamsClient = new MediaFileStreamsClient();
        }

        public ImageViewerModel GetImageViewerModel(Guid streamId, string fileType)
        {
            const int maxWidth = PreviewConstants.MaxImagePreviewgWidth;        
            var file = _mediaFileStreamsClient.Get(streamId);

            var originalSize = GetOriginalSize(file);
            var size = GetImageSize(originalSize.Width, originalSize.Height);

            var paddingLeft = (size.Width < maxWidth)
                ? $"{(maxWidth - size.Width) / 2}px" : "0";

            var base64String = Convert.ToBase64String(file.Stream).TrimStart('\"').TrimEnd('\"');
            var prefix = $"data:image/{file.FileType.ToLower()};base64";

            return new ImageViewerModel
            {
                FileType = fileType,
                Width = size.Width,
                Height = size.Height,
                PaddingLeft = paddingLeft,
                Base64String = $"{prefix},{base64String}"
            };
        }

        private static ImageSize GetOriginalSize(MediaFileStream file)
        {
            int originalWidth;
            int originalHeight;
            var bytes = file.Stream;
            using (var stream = new MemoryStream(bytes, 0, bytes.Length))
            {
                using (var image = Image.FromStream(stream, false, false))
                {
                    originalWidth = image.Width;
                    originalHeight = image.Height;
                }
            }

            return new ImageSize { Width = originalWidth, Height = originalHeight };
        }

        private static ImageSize GetImageSize(int width, int height)
        {
            int newWidth;
            int newHeight;

            const int dialogWidth = PreviewConstants.MaxImagePreviewgWidth;
            const int dialogHeight = PreviewConstants.MaxImagePreviewHeight;
            const int dialognRatio = dialogWidth / dialogHeight;

            var imageRatio = (double)width / height;

            // if the picture is landscape or a perfect square
            if (imageRatio >= 1)
            {
                // if the picture is "more square" than the screen
                if (imageRatio < dialognRatio)
                {
                    newHeight = dialogHeight;
                    newWidth = (int)(newHeight * imageRatio);
                }

                // if the picture is "less square" than the screen
                else
                {
                    newWidth = dialogWidth;
                    newHeight = (int)(newWidth / imageRatio);
                }
            }

            // the picture is portrait
            else
            {
                newHeight = dialogHeight;
                newWidth = (int)(newHeight * imageRatio);
            }

            return new ImageSize { Width = newWidth, Height = newHeight };
        }
    }
}
