using System.Web.Optimization;

namespace IOM
{
    public static class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.ajax.js",
                        "~/Scripts/additional-methods.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/iom-apiauth").Include(
                        "~/Scripts/jquery.signalR-2.4.1.min.js",
                        "~/Scripts/knockout-3.5.0.js",
                        "~/Scripts/Util/iom-jquery.plugins.js",
                        "~/Scripts/Util/iom-apiauth.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/umd/popper.min.js",
                        "~/Scripts/jquery-ui-1.12.1.min.js",
                        "~/Content/bootstrap-select/js/bootstrap-select.js",
                        "~/Scripts/bootstrap.min.js"));
            
            bundles.Add(new StyleBundle("~/Content/css-login").Include(
                        "~/Content/bootstrap.min.css",
                        "~/admin-lte/css/AdminLTE.min.css",
                        "~/admin-lte/css/skins/skin-blue.min.css",
                        "~/admin-lte/plugin/iCheck/flat/green.css",
                        "~/admin-lte/plugin/iCheck/minimal/blue.css",
                        "~/admin-lte/plugin/iCheck/square/green.css",
                        "~/Content/Site.css"));
             
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/font-awesome.min.css",
                        "~/admin-lte/plugin/select2/dist/css/select2.min.css",
                        "~/admin-lte/css/AdminLTE.css",
                        "~/admin-lte/css/skins/skin-blue.css",
                        "~/admin-lte/plugin/datatables.net-bs/css/dataTables.boostrap.min.css",
                        "~/admin-lte/plugin/iCheck/square/green.css",
                        "~/admin-lte/plugin/iCheck/all.css",
                        "~/admin-lte/plugin/js-grid/jsgrid-theme.min.css",
                        "~/admin-lte/plugin/js-grid/jsgrid.min.css",
                        "~/admin-lte/plugin/jquery.jsonPresenter/jquery.jsonPresenter.css",
                        "~/Content/bootstrap-select/css/bootstrap-select.css",
                        "~/Content/daterangepicker.css",
                        "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/icheck").Include(
                        "~/admin-lte/plugin/iCheck/icheck.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                        "~/admin-lte/plugin/moment/min/moment.min.js",
                        "~/admin-lte/plugin/datatables.net/js/jquery.dataTables.min.js",
                        "~/admin-lte/plugin/datatables.net-bs/js/dataTables.bootstrap.min.js",
                        "~/admin-lte/plugin/jquery-slimscroll/jquery.slimscroll.min.js",
                        "~/admin-lte/plugin/fastclick/lib/fastclick.js",
                        "~/admin-lte/plugin/iCheck/icheck.min.js",
                        "~/admin-lte/plugin/js-grid/jsgrid.min.js",
                        "~/admin-lte/plugin/jquery.jsonPresenter/jquery.jsonPresenter.js",
                        "~/admin-lte/plugin/jsPDF/jspdf.debug.js",
                        "~/admin-lte/plugin/notify/notify.js",
                        "~/admin-lte/plugin/validate.js/validate.js",
                        "~/admin-lte/plugin/jquery.blockUI/jquery.blockUI.js",
                        "~/admin-lte/plugin/jspdf_autotable/jspdf.plugin.autotable.js",
                        "~/Scripts/validator/validator.min.js",
                        "~/admin-lte/plugin/select2/dist/js/select2.full.min.js",
                        "~/admin-lte/js/adminlte.min.js",
                        "~/Scripts/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/jquery.signalR-2.4.1.min.js",
                        "~/Scripts/knockout-3.5.0.js",
                        "~/Scripts/Util/iom-jquery.plugins.js",
                        "~/Scripts/Util/iom-apiauth.js",
                        "~/Scripts/site.js"));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
