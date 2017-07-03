using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.BusinessRules.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Keebee.AAT.BusinessRules
{
    public class ImageViewerRules
    {
        public static class PreviewConstants
        {
            public const int MaxImagePreviewgWidth = 500;
            public const int MaxImagePreviewHeight = 500;
        }

        private struct ImageSize
        {
            public int Width;
            public int Height;
        }

        private readonly IMediaFileStreamsClient _mediaFileStreamsClient;

        public ImageViewerRules()
        {
            _mediaFileStreamsClient = new MediaFileStreamsClient();
        }

        public ImageViewerModel GetImageViewerModel(Guid streamId, string fileType)
        {   
            var file = _mediaFileStreamsClient.Get(streamId);

            // get image from stream
            Image image;
            using (var ms = new MemoryStream(file.Stream))
            {
                image = Image.FromStream(ms);
            }

            // figure out the best size for the image viewer dimensions
            var newSize = GetImageSize(image.Width, image.Height);

            // resize the image
            var resizedImage = (Image) new Bitmap(image, 
                new Size {Height = newSize.Height, Width = newSize.Width});

            // convert back to stream
            var stream = new MemoryStream();
            resizedImage.Save(stream, GetImageFormat(file.FileType));

            // convert to base64 string and generate the prefix
            var base64String = Convert.ToBase64String(stream.ToArray());
            var prefix = $"data:image/{file.FileType.ToLower()};base64";

            return new ImageViewerModel
            {
                FileType = fileType,
                Width = newSize.Width,
                Height = newSize.Height,
                Base64String = $"{prefix},{base64String}"
            };
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
                    newHeight = dialogHeight;
                    newWidth = (int)(newHeight * imageRatio);
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

        private static ImageFormat GetImageFormat(string fileType)
        {
            switch (fileType.ToLower())
            {
                case "gif":
                    return ImageFormat.Gif;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                default:
                    return ImageFormat.Jpeg;
            }
        }
    }
}
