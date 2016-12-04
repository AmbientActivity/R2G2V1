using System.IO;
using System.Linq;
using Keebee.AAT.Shared;

namespace Keebee.AAT.VideoCaptureFileCleanup
{
    internal class Program
    {
        private static void Main()
        {
            var root = new DirectoryInfo(VideoCaptures.Path);
            var folders = root.EnumerateDirectories().OrderBy(x => x.Name);

            var files = folders.SelectMany(x => x.EnumerateFiles()
                .Where(f => f.Length == 0)).Select(x => $@"{x.Directory}\{x.Name}");

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
