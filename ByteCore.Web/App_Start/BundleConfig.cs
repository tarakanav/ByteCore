using System.Web.Optimization;

namespace ByteCore.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/mainjs").Include(
                "~/Content/lib/jquery.min.js",
                "~/Content/lib/popper.min.js",
                "~/Content/lib/bootstrap.min.js",
                "~/Content/lib/jquery.slimscroll.js",
                "~/Content/lib/modernizr.js",
                "~/Content/lib/amcharts.min.js",
                "~/Content/lib/serial.min.js",
                "~/Content/lib/Chart.js",
                "~/Content/lib/todo.js",
                "~/Content/lib/i18next.min.js",
                "~/Content/lib/i18nextXHRBackend.min.js",
                "~/Content/lib/i18nextBrowserLanguageDetector.min.js",
                "~/Content/lib/jquery-i18next.min.js",
                "~/Content/lib/custom-dashboard.min.js",
                "~/Content/lib/SmoothScroll.js",
                "~/Content/lib/pcoded.min.js",
                "~/Content/lib/demo-12.js",
                "~/Content/lib/jquery.mCustomScrollbar.concat.min.js",
                "~/Content/lib/script.min.js"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}