using System;
using System.Drawing;
using System.IO;

namespace Keebee.AAT.Display.Caregiver.Thumbnail
{
    public static class Picture
    {
        public static Image Get(string fileName, int dimensions)
        {
            Bitmap bitmap;
            using (Stream bmpStream = File.Open(fileName, FileMode.Open))
            {
                var image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }

            return bitmap.GetThumbnailImage(dimensions, dimensions, () => true, new IntPtr());
        }
    }
}
