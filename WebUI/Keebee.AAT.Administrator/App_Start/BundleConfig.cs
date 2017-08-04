using System.Web.Optimization;

namespace Keebee.AAT.Administrator
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/Jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/JqueryVal").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/Modernizr").Include(
                "~/Scripts/modernizr-.*"));

            bundles.Add(new ScriptBundle("~/bundles/ThirdPartyScripts").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/simpleUpload.js",
                "~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/CustomScripts").Include(
                "~/Scripts/Site/Namespaces.js",
                "~/Scripts/Site/Site.js",
                "~/Scripts/Account/Services.js",
                "~/Scripts/Residents/Index.js",
                "~/Scripts/Residents/_ResidentEdit.js",
                "~/Scripts/ResidentProfile/Index.js",
                "~/Scripts/EventLogs/Index.js",
                "~/Scripts/EventLogs/Export.js",
                "~/Scripts/VideoCaptures/Index.js",
                "~/Scripts/PhidgetConfig/Index.js",
                "~/Scripts/PhidgetConfig/_ConfigDetailEdit.js",
                "~/Scripts/SharedLibrary/Index.js",
                "~/Scripts/SharedLibrary/_LinkedProfiles.js",
                "~/Scripts/SharedLibraryAdd/_Index.js",
                "~/Scripts/SharedLibraryAdd/PartialView.js",
                "~/Scripts/SystemProfile/Index.js",
                "~/Scripts/PublicProfile/Index.js",
                "~/Scripts/Services/Index.js",
                "~/Scripts/Utilities/InProgress.js",
                "~/Scripts/Utilities/Confirm.js",
                "~/Scripts/Utilities/Alert.js",
                "~/Scripts/Utilities/PartialView.js",
                "~/Scripts/Utilities/JobExecution.js",
                "~/Scripts/Utilities/Sorting.js",
                "~/Scripts/Utilities/ImageViewer.js",
                "~/Scripts/Utilities/VideoViewer.js",
                "~/Scripts/Utilities/Validation.js",
                "~/Scripts/Utilities/Upload.js",
                "~/Scripts/Utilities/SessionExpiry.js",
                "~/Scripts/Maintenance/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/Bootstrap").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/bootstrap-dialog.js",
                "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new StyleBundle("~/bundles/CommonStyles").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-datepicker.css",
                "~/Content/bootstrap-dialog.css",
                "~/Content/font-awesome.css",
                "~/Content/foundation-icons/foundation-icons.css",
                "~/Content/site.css",
                "~/Content/Buttons.css"));
        }
    }
}