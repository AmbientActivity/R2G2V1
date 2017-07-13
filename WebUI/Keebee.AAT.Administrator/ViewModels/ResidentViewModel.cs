using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class ResidentsViewModel
    {
        public int SelectedId { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool Success { get; set; }
        public string IdSearch { get; set; }
        public string FirstNameSearch { get; set; }
        public string LastNameSearch { get; set; }
        public string SortColumnName { get; set; }
        public int? SortDescending { get; set; }
        public int IsVideoCaptureServiceInstalled { get; set; }
}

    public class ResidentViewModel
    {
        public int Id { get; set; }
        public int? ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int GameDifficultyLevel { get; set; }
        public bool AllowVideoCapturing { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePicturePlaceholder { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class ResidentEditViewModel : ResidentViewModel
    {
        public SelectList GameDifficultyLevels { get; set; }
        public bool IsVideoCaptureServiceInstalled { get; set; }
    }

    public class SharedLibraryLinkViewModel
    {
        public IEnumerable<SharedLibraryFileViewModel> SharedFiles { get; set; }
    }

    public class SharedLibraryAddViewModel
    {
        public int ProfileId { get; set; }
        public int MediaPathTypeId { get; set; }
        public string MediaPathTypeDesc { get; set; }
        public string MediaPathTypeCategory { get; set; }
    }

    public class ResidentProfileViewModel
    {
        public int ResidentId { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string IdSearch { get; set; }
        public string FirstNameSearch { get; set; }
        public string LastNameSearch { get; set; }
        public string SortColumn { get; set; }
        public int? SortDescending { get; set; }
        public int? SelectedMediaPathType { get; set; }
        public int? SelectedMediaSourceType { get; set; }
        public string UploaderHtml { get; set; }
        public string ErrorMessage { get; set; }
    }
}