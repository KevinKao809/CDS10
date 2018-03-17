using System.Web;
using System.Web.Optimization;

namespace sfAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            /* SmartFactory */
            bundles.Add(new StyleBundle("~/sfAssets/css").Include(

                      "~/assets/css/bootstrap.min.css",
                      "~/assets/css/core.css",
                      "~/assets/css/components.css",
                      "~/assets/css/icons.css",
                      "~/assets/css/pages.css",
                      "~/assets/css/menu.css",
                      "~/assets/css/menu-customize.css",
                      "~/assets/css/responsive.css",
                      "~/assets/css/smartfactory.css",

                      "~/assets/plugins/toastr/toastr.min.css",
                      "~/assets/plugins/bootstrap-sweetalert/sweet-alert.css",
                      "~/assets/plugins/timepicker/bootstrap-timepicker.min.css",
                      "~/assets/plugins/bootstrap-tagsinput/dist/bootstrap-tagsinput.css",
                      "~/assets/plugins/multiselect/css/multi-select.css",
                      "~/assets/plugins/select2/dist/css/select2.css",
                      "~/assets/plugins/select2/dist/css/select2-bootstrap.css",
                      "~/assets/plugins/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.css",
                      "~/assets/plugins/switchery/switchery.min.css",
                      "~/assets/plugins/timepicker/bootstrap-timepicker.min.css",
                      "~/assets/plugins/mjolnic-bootstrap-colorpicker/dist/css/bootstrap-colorpicker.min.css",
                      "~/assets/plugins/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css",
                      "~/assets/plugins/bootstrap-daterangepicker/daterangepicker.css",
                      "~/assets/css/bootstrap-combined.min.css"
                      ));


            bundles.Add(new StyleBundle("~/sfAssets/css/datatables").Include(
                      "~/assets/plugins/datatables/jquery.dataTables.min.css",
                      "~/assets/plugins/datatables/buttons.bootstrap.min.css",
                      "~/assets/plugins/datatables/fixedHeader.bootstrap.min.css",
                      "~/assets/plugins/datatables/responsive.bootstrap.min.css",
                      "~/assets/plugins/datatables/scroller.bootstrap.min.css"                      
                      ));

            bundles.Add(new ScriptBundle("~/sfAssets/modernizr").Include(
                        "~/assets/js/modernizr.min.js"));

            bundles.Add(new ScriptBundle("~/sfAssets/jquery").Include(
                        "~/assets/js/jquery.min.js",
                        "~/assets/js/bootstrap.min.js",
                        "~/assets/js/detect.js",
                        "~/assets/js/fastclick.js",
                        "~/assets/js/jquery.slimscroll.js",
                        "~/assets/js/jquery.blockUI.js",
                        "~/assets/js/waves.js",
                        "~/assets/js/wow.min.js",
                        "~/assets/js/jquery.nicescroll.js",
                        "~/assets/js/jquery.scrollTo.min.js",
                        "~/assets/js/jquery.core.js",
                        "~/assets/js/jquery.app.js",
                        "~/assets/js/smartfactory.js",

                        "~/assets/plugins/switchery/switchery.min.js",
                        "~/assets/plugins/bootstrap-tagsinput/dist/bootstrap-tagsinput.min.js",
                        "~/assets/plugins/multiselect/js/jquery.multi-select.js",
                        "~/assets/plugins/jquery-quicksearch/jquery.quicksearch.js",
                        "~/assets/plugins/select2/dist/js/select2.min.js",
                        "~/assets/plugins/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.js",
                        "~/assets/plugins/bootstrap-inputmask/bootstrap-inputmask.min.js",
                        "~/assets/plugins/moment/moment.js",
                        "~/assets/plugins/timepicker/bootstrap-timepicker.min.js",
                        "~/assets/plugins/mjolnic-bootstrap-colorpicker/dist/js/bootstrap-colorpicker.min.js",
                        "~/assets/plugins/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                        "~/assets/plugins/bootstrap-daterangepicker/daterangepicker.js",
                        "~/assets/plugins/bootstrap-maxlength/bootstrap-maxlength.min.js",

                        "~/assets/plugins/toastr/toastr.min.js",
                        "~/assets/plugins/bootstrap-sweetalert/sweet-alert.min.js",
                        "~/assets/plugins/jscolor/jscolor.js"
                        ));

            bundles.Add(new ScriptBundle("~/sfAssets/js/datatables").Include(
                        "~/assets/plugins/datatables/jquery.dataTables.min.js",
                        "~/assets/plugins/datatables/dataTables.bootstrap.js",
                        "~/assets/plugins/datatables/dataTables.buttons.min.js",
                        "~/assets/plugins/datatables/buttons.bootstrap.min.js",
                        "~/assets/plugins/datatables/jszip.min.js",
                        "~/assets/plugins/datatables/pdfmake.min.js",
                        "~/assets/plugins/datatables/vfs_fonts.js",
                        "~/assets/plugins/datatables/buttons.html5.min.js",
                        "~/assets/plugins/datatables/buttons.print.min.js",
                        "~/assets/plugins/datatables/dataTables.fixedHeader.min.js",
                        "~/assets/plugins/datatables/dataTables.keyTable.min.js",
                        "~/assets/plugins/datatables/dataTables.responsive.min.js",
                        "~/assets/plugins/datatables/responsive.bootstrap.min.js",
                        "~/assets/plugins/datatables/dataTables.scroller.min.js",
                        "~/assets/pages/datatables.init.js"
                        ));

            /* End SmartFactory */
        }
    }
}
