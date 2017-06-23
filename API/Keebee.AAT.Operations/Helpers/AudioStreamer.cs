using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Keebee.AAT.Operations.Helpers
{
    public class AudioStreamer
    {
        private readonly string _filepath;

        public AudioStreamer(string filepath)
        {
            _filepath = filepath;
        }

        public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];

                using (var video = File.Open(_filepath, FileMode.Open, FileAccess.Read))
                {
                    var length = 5000000;
                    if (video.Length < 5000000)
                    {
                        length = (int)video.Length;
                    }
                    //var length = (int)video.Length;
                    var bytesRead = 1;
                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
            catch (HttpException ex)
            {
                Console.Write(ex.Message);
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}