using System.Web.Optimization;
using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;

namespace Keebee.AAT.Administrator
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var nullBuilder = new NullBuilder();
            var styleTransformer = new StyleTransformer();
            var scriptTransformer = new ScriptTransformer();

            // Replace a default bundle resolver in order to the debugging HTTP-handler
            // can use transformations of the corresponding bundle
            BundleResolver.Current = new CustomBundleResolver();

            var jQueryBundle = new Bundle("~/bundles/Jquery");
            jQueryBundle.Include(
                "~/Scripts/jquery-{version}.js");
            jQueryBundle.Builder = nullBuilder;
            jQueryBundle.Transforms.Add(scriptTransformer);
            bundles.Add(jQueryBundle);

            var jQueryValBundle = new Bundle("~/bundles/JqueryVal");
            jQueryValBundle.Include(
                "~/Scripts/jquery.validate*");
            jQueryValBundle.Builder = nullBuilder;
            jQueryValBundle.Transforms.Add(scriptTransformer);
            bundles.Add(jQueryValBundle);

            var thirdPartyScriptBundle = new Bundle("~/bundles/ThirdPartyScripts");
            thirdPartyScriptBundle.Include(
                "~/Scripts/knockout-{version}.js",
                 "~/Scripts/moment.js");
            thirdPartyScriptBundle.Builder = nullBuilder;
            thirdPartyScriptBundle.Transforms.Add(scriptTransformer);
            bundles.Add(thirdPartyScriptBundle);

            var customScriptBundle = new ScriptBundle("~/bundles/CustomScripts");
            customScriptBundle.Include(
                "~/Scripts/Site/Namespaces.js",
                "~/Scripts/Site/Site.js",
                "~/Scripts/Residents/Index.js",
                "~/Scripts/Profiles/Index.js",
                "~/Scripts/Profiles/Edit.js",
                "~/Scripts/EventLogs/Index.js",
                "~/Scripts/EventLogs/Export.js",
                "~/Scripts/Configurations/Index.js",
                "~/Scripts/MediaFiles/Index.js");
            customScriptBundle.Builder = nullBuilder;
            customScriptBundle.Transforms.Add(scriptTransformer);
            bundles.Add(customScriptBundle);

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            var modernizrBundle = new Bundle("~/bundles/Modernizr");
            modernizrBundle.Include("~/Scripts/modernizr-.*");
            modernizrBundle.Builder = nullBuilder;
            modernizrBundle.Transforms.Add(scriptTransformer);
            bundles.Add(modernizrBundle);

            var bootstrapBundle = new Bundle("~/bundles/Bootstrap");
            bootstrapBundle.Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-dialog.js",
                      "~/Scripts/bootstrap-datepicker.js");
            bootstrapBundle.Builder = nullBuilder;
            bootstrapBundle.Transforms.Add(scriptTransformer);
            bundles.Add(bootstrapBundle);

            var commonStylesBundle = new StyleBundle("~/bundles/CommonStyles");
            commonStylesBundle.Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-datepicker.css",
                "~/Content/bootstrap-dialog.css",
                "~/Content/site.css");
            commonStylesBundle.Builder = nullBuilder;
            commonStylesBundle.Transforms.Add(styleTransformer);
            bundles.Add(commonStylesBundle);

            // uncomment to see optimizations locally
            //BundleTable.EnableOptimizations = true;
        }
    }
}