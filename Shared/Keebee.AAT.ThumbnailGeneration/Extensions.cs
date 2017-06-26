using System;
using System.Drawing;
using System.Web.Helpers;

namespace Keebee.AAT.ThumbnailGeneration
{
    public static class Extensions
    {
        public static WebImage CustomCrop(this WebImage image, decimal targetRatio)
        {
            var currentImageRatio = image.Width / (decimal)image.Height;
            int difference;

            //image is wider than targeted
            if (currentImageRatio > targetRatio)
            {
                var targetWidth = Convert.ToInt32(Math.Floor(targetRatio * image.Height));
                difference = image.Width - targetWidth;
                var left = Convert.ToInt32(Math.Floor(difference / (decimal)2));
                var right = Convert.ToInt32(Math.Ceiling(difference / (decimal)2));
                image.Crop(0, left, 0, right);
            }
            //image is higher than targeted
            else if (currentImageRatio < targetRatio)
            {
                var targetHeight = Convert.ToInt32(Math.Floor(image.Width / targetRatio));
                difference = image.Height - targetHeight;
                var top = Convert.ToInt32(Math.Floor(difference / (decimal)2));
                var bottom = Convert.ToInt32(Math.Ceiling(difference / (decimal)2));
                image.Crop(top, 0, bottom);
            }
            return image;
        }

        public static Image Orient(this Image image)
        {
            foreach (var prop in image.PropertyItems)
            {
                if ((prop.Id != 0x0112 && prop.Id != 5029 && prop.Id != 274)) continue;

                var value = (int)prop.Value[0];
                if (value == 6)
                {
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                }
                if (value == 8)
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                }
                if (value != 3) continue;
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                break;
            }

            return image;
        }
    }
}

