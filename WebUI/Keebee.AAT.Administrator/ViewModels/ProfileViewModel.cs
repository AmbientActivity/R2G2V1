using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class ProfileDetail
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int ResidentId { get; set; }
        public int GameDifficultyLevel { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class ProfileViewModel
    {
        public int SelectedId { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool Success { get; set; }
    }

    public class ProfileEditViewModel
    {
        public int Id { get; set; }
        public int ResidentId { get; set; }
        public string Title { get; set; }
    }
}