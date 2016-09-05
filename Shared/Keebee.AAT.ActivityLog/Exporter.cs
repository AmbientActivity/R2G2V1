using Keebee.AAT.RESTClient;
using Keebee.AAT.EventLogging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExcelLibrary.SpreadSheet;

namespace Keebee.AAT.ActivityLog
{
    public class Exporter
    {
        private readonly OperationsClient _opsClient;
        private readonly EventLogger _eventLogger;

        public Exporter()
        {
            _opsClient = new OperationsClient();
            _eventLogger = new EventLogger(EventLogType.ActivityLog);
        }

        public byte[] Export(string date)
        {
            var workbook = GetWorkbook(date);
            var m = new MemoryStream();
            workbook.SaveToStream(m);
            var file = m.ToArray();

            return file;
        }

        public void ExportAndSave(string date)
        {
            var workbook = GetWorkbook(date);

            var filename = $"{Constants.ActivityLog.Path}\\ActivityLog_{date.Replace("/", "_")}_{DateTime.Now.Ticks}.xls";

            workbook.Save(filename);
        }

        private Workbook GetWorkbook(string date)
        {
            var workbook = new Workbook();

            try
            {
                var eventLogs = _opsClient.GetEventLogs(date).ToArray();
                var gamingEventLogs = _opsClient.GetGamingEventLogs(date).ToArray();

                var worksheet1 = new Worksheet("Activity Log");

                for (var i = 0; i < 100; i++)
                    worksheet1.Cells[i, 0] = new Cell("");

                worksheet1.Cells[0, 0] = new Cell("Date") { Format = CellFormat.Date };
                worksheet1.Cells[0, 1] = new Cell("Time") { Format = CellFormat.Date };
                worksheet1.Cells[0, 2] = new Cell("RFID");
                worksheet1.Cells[0, 3] = new Cell("Resident");
                worksheet1.Cells[0, 4] = new Cell("Activity");
                worksheet1.Cells[0, 5] = new Cell("Response Type");
                worksheet1.Cells[0, 6] = new Cell("Description");

                ushort rowIndex = 1;
                foreach (var eventLog in eventLogs)
                {
                    worksheet1.Cells[rowIndex, 0] = new Cell(eventLog.Date);
                    worksheet1.Cells[rowIndex, 1] = new Cell(eventLog.Time);
                    worksheet1.Cells[rowIndex, 2] = new Cell(eventLog.ResidentId);
                    worksheet1.Cells[rowIndex, 3] = new Cell(eventLog.Resident);
                    worksheet1.Cells[rowIndex, 4] = new Cell(eventLog.ActivityType);
                    worksheet1.Cells[rowIndex, 5] = new Cell(eventLog.ResponseTypeCategory);
                    worksheet1.Cells[rowIndex, 6] = new Cell(eventLog.Description);

                    rowIndex++;
                }

                workbook.Worksheets.Add(worksheet1);

                var worksheet2 = new Worksheet("Gaming Log")
                {
                    Cells =
                                     {
                                         [0, 0] = new Cell("Date"),
                                         [0, 1] = new Cell("Time"),
                                         [0, 2] = new Cell("RFID"),
                                         [0, 3] = new Cell("Resident"),
                                         [0, 4] = new Cell("Difficulty"),
                                         [0, 5] = new Cell("Success"),
                                         [0, 6] = new Cell("Description")
                                     }
                };

                rowIndex = 1;
                foreach (var eventLog in gamingEventLogs)
                {
                    worksheet2.Cells[rowIndex, 0] = new Cell(eventLog.Date) { Format = CellFormat.Date };
                    worksheet2.Cells[rowIndex, 1] = new Cell(eventLog.Time) { Format = CellFormat.Date };
                    worksheet2.Cells[rowIndex, 2] = new Cell(eventLog.ResidentId);
                    worksheet2.Cells[rowIndex, 3] = new Cell(eventLog.Resident);
                    worksheet2.Cells[rowIndex, 4] = new Cell(eventLog.DifficultyLevel);
                    if (eventLog.IsSuccess != null)
                        worksheet2.Cells[rowIndex, 5] = new Cell(eventLog.IsSuccess.ToString().ToUpper());
                    else
                        worksheet2.Cells[rowIndex, 5] = new Cell(string.Empty);
                    worksheet2.Cells[rowIndex, 6] = new Cell(eventLog.Description);

                    rowIndex++;
                }

                workbook.Worksheets.Add(worksheet2);
            }
            catch (Exception ex)
            {
                _eventLogger?.WriteEntry($"ActivityLog.GetWorkbook: {ex.Message}", EventLogEntryType.Error);
            }

            return workbook;
        }
    }
}