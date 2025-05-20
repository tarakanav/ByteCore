using System.Web.Optimization;

namespace ByteCore.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/mainjs").Include(
                "~/lib/jquery/js/jquery.min.js",
                "~/lib/popper.js/js/popper.min.js",
                "~/lib/bootstrap/js/bootstrap.min.js",
                "~/lib/jquery-slimscroll/js/jquery.slimscroll.js",
                "~/lib/modernizr/js/modernizr.js",
                "~/lib/amchart/amcharts.min.js",
                "~/lib/amchart/serial.min.js",
                "~/lib/chart.js/js/Chart.js",
                "~/lib/todo/todo.js",
                "~/lib/i18next/js/i18next.min.js",
                "~/lib/i18next-xhr-backend/js/i18nextXHRBackend.min.js",
                "~/lib/i18next-browser-languagedetector/js/i18nextBrowserLanguageDetector.min.js",
                "~/lib/jquery-i18next/js/jquery-i18next.min.js",
                "~/lib/dashboard/custom-dashboard.min.js",
                "~/lib/SmoothScroll.js",
                "~/lib/pcoded.min.js",
                "~/lib/demo-12.js",
                "~/lib/jquery.mCustomScrollbar.concat.min.js",
                "~/lib/script.min.js"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}