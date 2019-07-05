using System.IO;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Extensions
{
    public static class ControllerExtensions
    {
        public static string RenderPartialViewToString(this ControllerBase controller, string partialPath, object model)
        {
            if (string.IsNullOrEmpty(partialPath))
                partialPath = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialPath);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                // copy model state items to the html helper 
                foreach (var item in viewContext.Controller.ViewData.ModelState)
                {
                    if (!viewContext.ViewData.ModelState.Keys.Contains(item.Key))
                    {
                        viewContext.ViewData.ModelState.Add(item);
                    }
                }

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}