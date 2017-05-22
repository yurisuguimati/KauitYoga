using System.Web;
using System.Web.Optimization;

namespace KaiutYoga
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.9.2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryconfirm").Include(
                        "~/Scripts/jquery.confirm.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvisible").Include(
                        "~/Scripts/jquery-visible.min.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/bootstrap/js/bootstrap.js",
                      "~/Content/bootstrap/js/bootstrap-switch.min.js",
                      "~/Scripts/respond.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap/css/bootstrap.css",
                    "~/Content/bootstrap/css/bootstrap-responsive.css",
                    "~/Content/bootstrap/css/bootstrap-switch.min.css",
                    "~/Content/site.css",
                    "~/Content/jquery-ui/jquery-ui.css"));
        }
    }
}
