using System.Collections.Generic;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class SystemLibrariesViewModel
    {
        public string Title { get; set; }
        public string AddButtonText { get; set; }
        public int? SelectedMediaPathType { get; set; }
        public string UploaderHtml { get; set; }
        public string UploadedMessage { get; set; }
    }

    public class LinkedProfilesViewModel
    {
        public IEnumerable<ResidentViewModel> Profiles { get; set; }
        public string NoAvailableMediaMessage { get; set; }
    }
}