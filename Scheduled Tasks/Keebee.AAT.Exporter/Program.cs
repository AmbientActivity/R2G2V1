using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Keebee.AAT.EventLogExporter
{
    internal class Program
    {
        private static void Main()
        {
            var sysEventLogger = new SystemEventLogger(SystemEventLogType.AutomatedExport);

            try
            {
                var date = DateTime.Now.AddDays(-1).ToString("d", DateTimeFormatInfo.InvariantInfo);

                var exporter = new Exporting.EventLogExporter();
                exporter.ExportAndSave(date);

                sysEventLogger.WriteEntry($"Event Log successfully exported for date: {date}");
            }
            catch (Exception ex)
            {
                sysEventLogger.WriteEntry($"EventLogExporter.Main: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
