using Keebee.AAT.RESTClient;
using System.Drawing;

namespace Keebee.AAT.BusinessRules
{
    public class ImagePreviewRules
    {
        public static class PreviewConstants
        {
            public const int MaxImagePreviewgWidth = 437;
            public const int MaxImagePreviewHeight = 333;
        }

        public struct ImageSize
        {
            public int Width;
            public int Height;
        }

        public ImageSize GetOriginalSize(MediaFileSingle file)
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

        public static ImageSize GetImageSize(int width, int height)
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
