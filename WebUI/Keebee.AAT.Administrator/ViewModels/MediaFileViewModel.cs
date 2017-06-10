using System;
using System.Collections.Generic;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class MediaFilesViewModel
    {
        public int SelectedId { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool Success { get; set; }
    }

    public class MediaFileViewModel
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
        public int MediaPathTypeId { get; set; }
        public bool IsLinked { get; set; }
    }

    public class SharedLibraryFileViewModel
    {
        public Guid StreamId { get; set; }
        public string Filename { get; set; }
    }
}