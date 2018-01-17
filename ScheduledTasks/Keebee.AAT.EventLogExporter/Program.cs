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
            try
            {
                var date = DateTime.Now.AddDays(-1).ToString("d", DateTimeFormatInfo.InvariantInfo);

                var exporter = new Exporting.EventLogExporter();
                exporter.ExportAndSave(date);

                SystemEventLogger.WriteEntry($"Event Log successfully exported for date: {date}", SystemEventLogType.AutomatedExport);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"EventLogExporter.Main: {ex.Message}", SystemEventLogType.AutomatedExport, EventLogEntryType.Error);
            }
        }
    }
}
