using System.Web;
using System.Web.Optimization;

namespace PosWarehouse
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/plugins/jquery.min.js"));
            //"~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins/scripts").Include(
                "~/plugins/jquery-migrate.min.js",
                "~/plugins/jquery-ui/jquery-ui.min.js",
                "~/plugins/bootstrap/js/bootstrap.min.js",
                "~/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                "~/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/plugins/jquery.blockui.min.js",
                "~/plugins/jquery.cokie.min.js",
                "~/plugins/uniform/jquery.uniform.min.js",
                "~/plugins/toastr/toastr.min.js",
                "~/plugins/toastr/ui-toastr.js",
                "~/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                //"~/plugins/scripts/wysihtml5-0.3.0.js",
                //"~/plugins/scripts/bootstrap-wysihtml5.js",
                "~/plugins/scripts/jquery.bootstrap.wizard.min.js",
                "~/plugins/scripts/select2.min.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/typeahead.js",
                "~/plugins/scripts/jquery.dataTables.min.js",
                "~/plugins/scripts/dataTables.tableTools.min.js",
                "~/plugins/scripts/dataTables.colReorder.min.js",
                "~/plugins/scripts/dataTables.scroller.min.js",
                "~/plugins/scripts/dataTables.bootstrap.js",
                "~/plugins/jquery-multi-select/js/jquery.multi-select.js",
                "~/plugins/jquery-multi-select/components-dropdowns.js",
                "~/plugins/scripts/metronic.js",
                "~/plugins/scripts/layout.js",
                "~/plugins/scripts/demo.js",
                //"~/plugins/scripts/charts-amcharts.js",
                //"~/plugins/scripts/charts-flotcharts.js",
                "~/plugins/scripts/table-managed.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/Assets/css").Include(
                      "~/plugins/font-awesome/css/font-awesome.min.css",
                      "~/plugins/simple-line-icons/simple-line-icons.min.css",
                      "~/plugins/bootstrap/css/bootstrap.min.css",
                      "~/plugins/uniform/css/uniform.default.css",
                      "~/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                      //"~/plugins/css/bootstrap-wysihtml5.css",
                      "~/plugins/toastr/toastr.min.css",
                      "~/plugins/css/select2.css",
                      "~/Content/jquery-ui.css",
                      "~/plugins/css/components-rounded.css",
                      "~/plugins/css/dataTables.scroller.min.css",
                      "~/plugins/css/dataTables.colReorder.min.css",
                      "~/plugins/css/dataTables.bootstrap.css",
                      "~/plugins/jquery-multi-select/css/multi-select.css",
                      "~/plugins/css/plugins.css",
                      "~/plugins/css/layout.css",
                      "~/plugins/css/themes/light.css",
                      "~/plugins/css/custom.css",
                      "~/Content/site.css",
                      "~/Content/Assets/preLoader.css"));
        }
    }
}
