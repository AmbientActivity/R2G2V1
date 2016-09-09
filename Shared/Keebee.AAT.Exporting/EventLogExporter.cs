using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using ExcelLibrary.SpreadSheet;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Keebee.AAT.Exporting
{
    public class EventLogExporter
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        public EventLogExporter()
        {
            _opsClient = new OperationsClient();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.EventLog);
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

            var filename = $@"\\{Environment.MachineName}\{CustomEventLog.Path}\EventLog_{date.Replace("/", "_")}.xls";

            workbook.Save(filename);
        }

        private Workbook GetWorkbook(string date)
        {
            var workbook = new Workbook();

            try
            {
                var activityEventLogs = _opsClient.GetActivityEventLogs(date).ToArray();
                var gameEventLogs = _opsClient.GetGameEventLogs(date).ToArray();
                var rfidEventLogs = _opsClient.GetRfidEventLogs(date).ToArray();

                // -------------------- RFID Log------------------------

                var worksheet1 = new Worksheet("RFID Log");

                // unfortunate hack needed to make ExcelLibrary work (need to initialize with 100 blank cells)
                for (var i = 0; i < 100; i++)
                    worksheet1.Cells[i, 0] = new Cell("");

                worksheet1.Cells[0, 0] = new Cell("Date");
                worksheet1.Cells[0, 1] = new Cell("Time");
                worksheet1.Cells[0, 2] = new Cell("RFID");
                worksheet1.Cells[0, 3] = new Cell("Resident");
                worksheet1.Cells[0, 4] = new Cell("Description");

                ushort rowIndex = 1;
                foreach (var eventLog in rfidEventLogs)
                {
                    worksheet1.Cells[rowIndex, 0] = new Cell(eventLog.Date) { Format = CellFormat.Date };
                    worksheet1.Cells[rowIndex, 1] = new Cell(eventLog.Time) { Format = CellFormat.Date };
                    worksheet1.Cells[rowIndex, 2] = new Cell(eventLog.ResidentId);
                    worksheet1.Cells[rowIndex, 3] = new Cell(eventLog.Resident);
                    worksheet1.Cells[rowIndex, 4] = new Cell(eventLog.Description);

                    rowIndex++;
                }

                workbook.Worksheets.Add(worksheet1);

                // -------------------- Activity Log------------------------

                var worksheet2 = new Worksheet("Activity Log")
                                 {
                                     Cells =
                                     {
                                         [0, 0] = new Cell("Date") { Format = CellFormat.Date },
                                         [0, 1] = new Cell("Time") { Format = CellFormat.Date },
                                         [0, 2] = new Cell("RFID"),
                                         [0, 3] = new Cell("Resident"),
                                         [0, 4] = new Cell("Activity"),
                                         [0, 5] = new Cell("Response Type"),
                                         [0, 6] = new Cell("Description")
                                     }
                                 };

                rowIndex = 1;
                foreach (var eventLog in activityEventLogs)
                {
                    worksheet2.Cells[rowIndex, 0] = new Cell(eventLog.Date);
                    worksheet2.Cells[rowIndex, 1] = new Cell(eventLog.Time);
                    worksheet2.Cells[rowIndex, 2] = new Cell(eventLog.ResidentId);
                    worksheet2.Cells[rowIndex, 3] = new Cell(eventLog.Resident);
                    worksheet2.Cells[rowIndex, 4] = new Cell(eventLog.ActivityType);
                    worksheet2.Cells[rowIndex, 5] = new Cell(eventLog.ResponseTypeCategory);
                    worksheet2.Cells[rowIndex, 6] = new Cell(eventLog.Description);

                    rowIndex++;
                }

                workbook.Worksheets.Add(worksheet2);

                //-------------------- Game Log ------------------------

                var worksheet3 = new Worksheet("Game Log")
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
                foreach (var eventLog in gameEventLogs)
                {
                    worksheet3.Cells[rowIndex, 0] = new Cell(eventLog.Date) { Format = CellFormat.Date };
                    worksheet3.Cells[rowIndex, 1] = new Cell(eventLog.Time) { Format = CellFormat.Date };
                    worksheet3.Cells[rowIndex, 2] = new Cell(eventLog.ResidentId);
                    worksheet3.Cells[rowIndex, 3] = new Cell(eventLog.Resident);
                    worksheet3.Cells[rowIndex, 4] = new Cell(eventLog.DifficultyLevel);
                    if (eventLog.IsSuccess != null)
                        worksheet3.Cells[rowIndex, 5] = new Cell(eventLog.IsSuccess.ToString().ToUpper());
                    else
                        worksheet3.Cells[rowIndex, 5] = new Cell(string.Empty);
                    worksheet3.Cells[rowIndex, 6] = new Cell(eventLog.Description);

                    rowIndex++;
                }

                workbook.Worksheets.Add(worksheet3);

            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Log.GetWorkbook: {ex.Message}", EventLogEntryType.Error);
            }

            return workbook;
        }
    }
}