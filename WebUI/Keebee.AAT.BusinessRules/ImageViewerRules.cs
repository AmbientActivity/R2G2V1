using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using System;
using System.Drawing;

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

        public ImageViewerModel GetImageViewerModel(Guid streamId, string fileType)
        {
            const int maxWidth = PreviewConstants.MaxImagePreviewgWidth;
            var mediaFilesClient = new MediaFilesClient();
            var file = mediaFilesClient.Get(streamId);

            var originalSize = GetOriginalSize(file);
            var size = GetImageSize(originalSize.Width, originalSize.Height);

            var paddingLeft = (size.Width < maxWidth)
                ? $"{(maxWidth - size.Width) / 2}px" : "0";

            return new ImageViewerModel
            {
                FilePath = $@"{file.Path}\{file.Filename}",
                FileType = fileType,
                Width = size.Width,
                Height = size.Height,
                PaddingLeft = paddingLeft
            };
        }

        private static ImageSize GetOriginalSize(MediaFileSingle file)
        {
            int originalWidth;
            int originalHeight;

            using (var stream = System.IO.File.OpenRead($@"{file.Path}\{file.Filename}"))
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
