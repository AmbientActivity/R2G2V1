using System.Collections.Generic;
using System.Web.Mvc;

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
        public bool CanDelete { get; set; }
    }

    public class ConfigDetailViewModel
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string PhidgetType { get; set; }
        public string Description { get; set; }
        public string ResponseType { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ConfigEditViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class ConfigDetailEditViewModel
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Description { get; set; }
        public int PhidgetTypeId { get; set; }
        public SelectList PhidgetTypes { get; set; }
        public int ResponseTypeId { get; set; }
        public SelectList ResponseTypes { get; set; }
    }
}