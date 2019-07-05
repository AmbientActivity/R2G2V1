using System;
using System.IO;
using System.Linq;
using NReco.VideoConverter;

namespace Keebee.AAT.VideoConversion
{
    public static class Helpers
    {
        public static byte[] ConvertVideo(string filePath, out string errorMessage)
        {
            errorMessage = null;
            byte[] file = null;
            try
            {
                var ffMpeg = new FFMpegConverter();
                var stream = new MemoryStream();

                ffMpeg.ConvertMedia(filePath, stream, Format.mp4);

                file = stream.ToArray();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return file;
        }

    }
}
