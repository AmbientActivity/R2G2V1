using System.Drawing;
using System.IO;

namespace Keebee.AAT.Display.Caregiver.Helpers
{
    public static class ThumbnailHelper
    {
        public static Image GetImageFromByteArray(byte[] byteArray)
        {
            Bitmap image;

            using (var memoryStream = new MemoryStream(byteArray))
            {
                using (var newImage = Image.FromStream(memoryStream, true))
                {
                    image = new Bitmap(newImage);
                }
            }

            return image;
        }
    }
}
