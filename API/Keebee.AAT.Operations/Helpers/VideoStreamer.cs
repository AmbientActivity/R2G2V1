using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keebee.AAT.Operations.Helpers
{
    public class VideoStreamer
    {
        public FileInfo FileInfo { get; set; }
        public long Start { get; set; }
        public long End { get; set; }

        public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];
                using (var video = FileInfo.OpenRead())
                {
                    if (End == -1)
                    {
                        End = video.Length;
                    }
                    var position = Start;
                    var bytesLeft = End - Start + 1;
                    video.Position = Start;
                    while (position <= End)
                    {
                        var bytesRead = video.Read(buffer, 0, (int)Math.Min(bytesLeft, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        position += bytesRead;
                        bytesLeft = End - position + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // fail silently
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}