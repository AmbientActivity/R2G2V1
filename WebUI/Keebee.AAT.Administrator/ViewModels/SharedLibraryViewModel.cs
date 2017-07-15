using System.Collections.Generic;

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
        public IEnumerable<ResidentViewModel> Profiles { get; set; }
        public string NoAvailableMediaMessage { get; set; }
    }
}