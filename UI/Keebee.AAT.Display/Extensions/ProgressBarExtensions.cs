using System.Windows.Forms;

namespace Keebee.AAT.Display.Extensions
{
    public static class ProgressBarExtensions
    {
        public static void SetProgressNoAnimation(this ProgressBar pb, int value)
        {
            if (value == pb.Maximum)
            {
                // kill the animation
                pb.Maximum = 101;
                pb.Value = 101;
                pb.Maximum = 100;
                // set the value
                pb.Value = value;
            }
            else
            {
                pb.Value = value + 1;       // Move past
            }
            pb.Value = value;               // Move to correct value
        }
    }
}
