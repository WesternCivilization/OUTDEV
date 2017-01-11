using System.Web;
using System.Web.Optimization;

namespace Oze
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Themes/plugins/jQuery/jquery-2.2.3.min.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Themes/plugins/jQueryUI/jquery-ui.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/html5shiv.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));

            /*NamLD 10/01/2016 09:22:26*/
            bundles.Add(new ScriptBundle("~/bundles/oze").Include(
                      
                      /* cac ham co ban dung cho toan bo project*/
                      "~/Scripts/oze.base.js"));

            /*NamLD 10/01/2016 09:22:26*/
            bundles.Add(new StyleBundle("~/Content/oze").Include(
                      /* cac font chu su dung trong project*/
                      "~/Content/font-awesome.min.css",
                      "~/Content/ionicons.min.css",
                      //"~/Content/skin/AdminLTE.css",
                      //"~/Contentskin/skin-blue.css",
                      "~/Content/oze.css",
                      "~/Content/oze.fonts.css"));

            /*2016-11-09 11:35:28 NamLD
                Data table
             */
            bundles.Add(new StyleBundle("~/Content/datatable").Include(
                      "~/Themes/plugins/datatables/dataTables.bootstrap.css",
                      "~/Themes/plugins/datatables/extensions/Responsive/css/responsive.bootstrap.css",
                      //"~/Themes/plugins/datatables/extensions/Responsive/css/dataTables.responsive.css",
                      "~/Themes/plugins/datatables/extensions/Scroller/css/dataTables.scroller.min.css",
                      "~/Themes/plugins/iCheck/all.css",
                      "~/Content/notify/notify-metro.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                      "~/Themes/plugins/datatables/jquery.dataTables.js",
                      "~/Themes/plugins/datatables/dataTables.bootstrap.js",
                      "~/Themes/plugins/datatables/extensions/Responsive/js/dataTables.responsive.js",
                      "~/Themes/plugins/datatables/extensions/Responsive/js/responsive.bootstrap.js",
                      "~/Themes/plugins/datatables/extensions/Scroller/js/dataTables.scroller.js",
                      "~/Themes/plugins/iCheck/icheck.min.js",
                      "~/Scripts/notify/notify.min.js",
                      "~/Scripts/notify/notify-metro.js"
                      ));
            bundles.Add(new StyleBundle("~/Content/datepicker").Include(
                      "~/Themes/plugins/datepicker/datepicker3.css",
                      "~/Themes/plugins/colorpicker/bootstrap-colorpicker.min.css",
                      "~/Themes/plugins/timepicker/bootstrap-timepicker.min.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                      "~/Themes/plugins/datepicker/moment.js",
                      "~/Themes/plugins/daterangepicker/daterangepicker.js",
                      "~/Themes/plugins/datepicker/bootstrap-datepicker.js",
                      "~/Themes/plugins/datepicker/bootstrap-datepicker.js",
                      "~/Themes/plugins/colorpicker/bootstrap-colorpicker.min.js",
                      "~/Themes/plugins/timepicker/bootstrap-timepicker.min.js"
                      ));
        }
    }
}
