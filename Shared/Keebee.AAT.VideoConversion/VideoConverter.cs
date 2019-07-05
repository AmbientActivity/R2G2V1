using System;
using System.IO;
using System.Linq;
using NReco.VideoConverter;

namespace Keebee.AAT.VideoConversion
{
    public class ConversionProgressEventArgs : EventArgs
    {
        public TimeSpan Processed { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }

    public static class VideoConverter
    {
        public static event EventHandler ConversionProgress;

        public static byte[] Convert(string fullPath, out string errorMessage)
        {
            errorMessage = null;
            byte[] file = null;
            try
            {
                var ffMpeg = new FFMpegConverter();
                var stream = new MemoryStream();

                ffMpeg.ConvertProgress += ConvertProgress;
                ffMpeg.ConvertMedia(fullPath, stream, Format.mp4);
                file = stream.ToArray();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return file;
        }

        public static string Save(string path, string filename, byte[] image)
        {
            string errorMessage = null;

            try
            {
                if (image != null)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    File.WriteAllBytes(Path.Combine(path, filename), image);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public static string GetCodecName(string filePath, out string errorMessage)
        {
            errorMessage = null;
            var codec = string.Empty;

            try
            {
                var probe = new NReco.VideoInfo.FFProbe();
                var info = probe.GetMediaInfo(filePath);

                if (info.Streams.Any())
                {
                    var s = info.Streams.First();
                    codec = s.CodecName;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return codec;
        }

        private static void ConvertProgress(object sender, ConvertProgressEventArgs e)
        {
            var args = new ConversionProgressEventArgs
            {
                Processed = e.Processed,
                TotalDuration = e.TotalDuration
            };

            ConversionProgress?.Invoke(new object(), args);
        }
    }
}
