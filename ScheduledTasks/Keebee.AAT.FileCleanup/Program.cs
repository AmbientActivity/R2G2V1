using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Keebee.AAT.Shared;

namespace Keebee.AAT.FileCleanup
{
    internal class Program
    {
        private static void Main()
        {
            DeleteEmptyVideoCaptureFiles();
            DeleteMediaPlayerLibraryFiles();
        }
        private static void DeleteEmptyVideoCaptureFiles()
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
        private static void DeleteMediaPlayerLibraryFiles()
        {
            var databasePath = $@"C:\Users\{Environment.UserName}\AppData\Local\Microsoft\Media Player";
            var root = new DirectoryInfo(databasePath);
            var dbFiles = root.EnumerateFiles().Select(x => $@"{x.Directory}\{x.Name}");

            foreach (var file in dbFiles)
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
