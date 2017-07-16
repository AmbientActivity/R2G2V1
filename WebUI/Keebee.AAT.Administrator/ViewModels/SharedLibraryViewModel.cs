using System;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class SharedLibraryViewModel
    {
        public string Title { get; set; }
        public int SelectedMediaPathTypeId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class LinkedProfilesViewModel
    {
        public Guid StreamId { get; set; }
    }
}