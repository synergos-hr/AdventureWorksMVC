using System.Web;
using System.Web.Optimization;

namespace AdventureWorks.Web
{
    public class BundleConfig
    {
        private static string _kendoVersion = "2017.1.118";
        private static string _adminLteVersion = "2.4.5";

        private static readonly string _kendoCdnRoot = "http://kendo.cdn.telerik.com/" + _kendoVersion;
        private static readonly string _adminLteCdnRoot = "https://cdnjs.cloudflare.com/ajax/libs/admin-lte/" + _adminLteVersion;

        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            // Scripts

            bundles.Add(new ScriptBundle("~/bundles/jQuery", "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sweetalert", "https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/7.26.11/sweetalert2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/appLib").Include(
                "~/Scripts/lib/modernizr-*",
                //"~/Scripts/lib/require.js",
                "~/Scripts/lib/respond.js",
                "~/Scripts/lib/es6-promise/es6-promise.auto.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/lib/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/kendo", _kendoCdnRoot + "/js/kendo.web.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/kendoLocale").Include(
            //    "~/Scripts/Kendo/2017.1.118/cultures/kendo.culture.hr-HR.min.js",
            //    "~/Scripts/Kendo/2017.1.118/messages/kendo.messages.hr-HR.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminLte", _adminLteCdnRoot + "/js/adminlte.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/appJs").Include(
                "~/Scripts/Synergos/synergos.js",
                "~/Scripts/Synergos/synergos.kendo.js",
                "~/Scripts/Synergos/synergos.grid.js",
                "~/Scripts/Synergos/synergos.ui.js",
                "~/Scripts/main.js",
                "~/Scripts/user.js"));

            // Content

            bundles.Add(new StyleBundle("~/Content/bootstrap", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/css/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/css/fonts/font-awesome/css/css").Include("~/Content/css/fonts/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/css/ionicons/css/css").Include("~/Content/css/ionicons/css/ionicons.css"));

            bundles.Add(new StyleBundle("~/Content/kendoCommon", _kendoCdnRoot + "/styles/kendo.common.min.css"));
            bundles.Add(new StyleBundle("~/Content/kendoSkinMetro", _kendoCdnRoot + "/styles/kendo.metro.min.css"));
            bundles.Add(new StyleBundle("~/Content/kendoSkinSilver", _kendoCdnRoot + "/styles/kendo.silver.min.css"));

            bundles.Add(new StyleBundle("~/Content/adminLte", _adminLteCdnRoot + "/css/AdminLTE.min.css"));
            bundles.Add(new StyleBundle("~/Content/adminLteSkinBlueLight", _adminLteCdnRoot + "/css/skins/skin-blue-light.min.css"));
            bundles.Add(new StyleBundle("~/Content/adminLteSkinGreenLight", _adminLteCdnRoot + "/css/skins/skin-green-light.min.css"));

            bundles.Add(new StyleBundle("~/Content/sweetalert", "https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/7.26.11/sweetalert2.min.css"));

            bundles.Add(new StyleBundle("~/Content/app").Include("~/Content/site.css"));

            BundleTable.EnableOptimizations = true; // if CDN is used optimizations have to be enabled
        }
    }
}
