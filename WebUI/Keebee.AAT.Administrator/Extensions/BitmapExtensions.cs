using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Keebee.AAT.Shared;

namespace Keebee.AAT.Administrator.Extensions
{
    public static class BitmapExtensions
    {
        internal class Dimensions
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public static Bitmap Scale(this Image image)
        {
            var dimensions = GetDimensions(image);
            var destRect = new Rectangle(0, 0, dimensions.Width, dimensions.Height);
            var destImage = new Bitmap(image.Width, image.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private static Dimensions GetDimensions(Image image)
        {
            int newWidth;
            int newHeight;

            double screenRatio = ScreenSize.Width / (double)ScreenSize.Height;
            double imageRatio = image.Width / (double)image.Height;

            // if the picture is landscape or a perfect square
            if (imageRatio >= 1)
            {
                // if the picture is "more square" than the screen
                if (imageRatio < screenRatio)
                {
                    newHeight = ScreenSize.Height;
                    newWidth = Convert.ToInt32(newHeight * imageRatio);
                }

                // if the picture is "less square" than the screen
                else
                {
                    newWidth = ScreenSize.Width;
                    newHeight = Convert.ToInt32(newWidth / imageRatio);
                }
            }

            // the picture is portrait
            else
            {
                newHeight = ScreenSize.Height;
                newWidth = Convert.ToInt32(newHeight * imageRatio);
            }

            return new Dimensions {Width = newWidth, Height = newHeight };
        }
    }
}