using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace Keebee.AAT.BusinessRules
{
    public class VideoCaptureRules
    {
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        public byte[] GetZipFile(string date)
        {
            var file = new byte[0];

            try
            {
                var dateTime = DateTime.Parse(date);
                var folder = dateTime.ToString("yyyy-MM-dd");
                var root = new DirectoryInfo($@"{VideoCaptures.Path}\{folder}");
                var files = root.EnumerateFiles()
                    .Where(f => f.Length > 0)
                    .Select(f => $@"{f.DirectoryName}\{f.Name}")
                    .ToList();

                using (var zipFile = new ZipFile())
                {
                    using (var zipStream = new MemoryStream())
                    {

                        zipFile.AddFiles(files, false, "");
                        zipFile.Save(zipStream);

                        zipStream.Seek(0, SeekOrigin.Begin);
                        file = zipStream.ToArray();
                    }
                }
            }
            catch(Exception ex)
            {
                _systemEventLogger.WriteEntry($"GetZipFile: {ex.Message}", EventLogEntryType.Error);
            }

            return file;
        }
    }
}
