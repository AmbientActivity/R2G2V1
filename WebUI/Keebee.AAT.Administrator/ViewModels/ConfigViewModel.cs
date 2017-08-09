using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Keebee.AAT.BusinessRules.Models;

namespace Keebee.AAT.Administrator.ViewModels
{
    public class ConfigsViewModel
    {
        public int SelectedId { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool Success { get; set; }
    }

    public class ConfigViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveEventLog { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ConfigDetailViewModel
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public int SortOrder { get; set; }
        public string PhidgetType { get; set; }
        public string PhidgetStyleType { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ResponseType { get; set; }
        public bool IsSystem { get; set; }
        public bool CanEdit { get; set; }
    }

    public class ConfigEditViewModel : ConfigViewModel
    {
        public string SourceConfigName { get; set; }
    }

    public class ConfigDetailEditViewModel : ConfigDetailViewModel
    {
        public bool IsActive { get; set; }
        public int PhidgetTypeId { get; set; }
        public SelectList PhidgetTypes { get; set; }
        public int PhidgetStyleTypeId { get; set; }
        public SelectList PhidgetStyleTypes { get; set; }
        public int ResponseTypeId { get; set; }
        public SelectList ResponseTypes { get; set; }
        public bool IsAdd { get; set; }
        public int SelectedPhidgetStyleTypeId { get; set; }
        public string AllowedInputStyleTypes { get; set; }
        public string AllowedSensorStyleTypes { get; set; }
    }
}