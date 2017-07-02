using System;
using System.Collections.Generic;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class EventLogsViewModel
    {
        public int SelectedId { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool Success { get; set; }
    }

    public class EventLogViewModel
    {
        public Guid StreamId { get; set; }
        public bool IsFolder { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
        public DateTime FileDate { get; set; }
    }
}