using System.Web.Optimization;

namespace ByteCore.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/bootstrap.css",
                "~/Content/themify-icons.css",
                "~/Content/icofont.css",
                "~/Content/style.css",
                "~/Content/jquery.mCustomScrollbar.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/popper.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery-slimscroll.js",
                "~/Scripts/modernizr.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/amcharts.js",
                "~/Scripts/serial.js",
                "~/Scripts/Chart.js",
                "~/Scripts/todo.js",
                "~/Scripts/i18next.js",
                "~/Scripts/i18nextXHRBackend.js",
                "~/Scripts/i18nextBrowserLanguageDetector.js",
                "~/Scripts/jquery-i18next.js",
                "~/Scripts/custom-dashboard.min.js",
                "~/Scripts/SmoothScroll.js",
                "~/Scripts/pcoded.min.js",
                "~/Scripts/demo-12.js",
                "~/Scripts/jquery.mCustomScrollbar.concat.min.js",
                "~/Scripts/script.min.js"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}