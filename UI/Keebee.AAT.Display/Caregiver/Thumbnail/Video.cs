using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Keebee.AAT.Display.Caregiver.Thumbnail
{
    public static class Video
    {
        public static Image Get(string file, int waitTime, int position)
        {
            var player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };

            player.Open(new Uri(file));
            player.Pause();
            player.Position = TimeSpan.FromSeconds(position);

            // need to give MediaPlayer some time to load. 
            // the efficiency of the MediaPlayer depends upon the capabilities of the machine it is running on and 
            System.Threading.Thread.Sleep(waitTime*1000);

            // 120 = thumbnail width, 90 = thumbnail height and 96x96 = horizontal x vertical DPI
            var rtb = new RenderTargetBitmap(120, 90, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawVideo(player, new Rect(0, 0, 120, 90));
            }

            rtb.Render(dv);
            var duration = player.NaturalDuration;

            if (duration.HasTimeSpan)
            {
                var videoLength = (int) duration.TimeSpan.TotalSeconds;
            }

            var frame = BitmapFrame.Create(rtb).GetCurrentValueAsFrozen() as BitmapFrame;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(frame);

            var memoryStream = new MemoryStream();
            encoder.Save(memoryStream);

            var byteArray = memoryStream.GetBuffer();
            var ms = new MemoryStream(byteArray);
            var thumbnail = Image.FromStream(ms);
            player.Close();

            return thumbnail;
        }
    }
}
