using System.Drawing;

namespace Keebee.AAT.Administrator.Extensions
{
    public static class ImageExtensions
    {
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