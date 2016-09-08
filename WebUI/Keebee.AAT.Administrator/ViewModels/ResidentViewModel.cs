﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class ResidentDetail
    {
        public int Id { get; set; }
        public int? ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class ResidentViewModel
    {
        public int SelectedId { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool Success { get; set; }
    }

    public class ResidentEditViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SelectList Genders { get; set; }
    }
}