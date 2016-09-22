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

        public string RfidSearch { get; set; }
        public string FirstNameSearch { get; set; }
        public string LastNameSearch { get; set; }
        public string SortColumnName { get; set; }
        public int? SortDescending { get; set; }
    }

    public class ResidentViewModel
    {
        public int Id { get; set; }
        public int? ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int GameDifficultyLevel { get; set; }
        public bool HasProfile { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class ResidentEditViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public SelectList Genders { get; set; }
        public int GameDifficultyLevel { get; set; }
        public SelectList GameDifficultyLevels { get; set; }
    }

    public class ResidentMediaViewModel
    {
        public int ResidentId { get; set; }
        public string FullName { get; set; }
        public string AddButtonText { get; set; }
        public string RfidSearch { get; set; }
        public string FirstNameSearch { get; set; }
        public string LastNameSearch { get; set; }
        public string SortColumn { get; set; }
        public int? SortDescending { get; set; }
        public int? SelectedMediaPathType { get; set; }
        public int? SelectedMediaSourceType { get; set; }
        public string UploaderHtml { get; set; }
        public string UploadedMessage { get; set; }
    }
}