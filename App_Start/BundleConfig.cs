using System.Web;
using System.Web.Optimization;

namespace WEBSGI
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
              "~/Content/bootstrap.css",
              "~/Content/site.css"));

            // Bundle para Font Awesome
            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                        "~/Content/font-awesome/css/font-awesome.min.css"));

          
            bundles.Add(new StyleBundle("~/Content/SidebarNav").Include(
                        "~/Content/SidebarNav.min.css"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-3.7.0.js"));
            bundles.UseCdn = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
      "~/Scripts/jquery-3.7.0.js"));



            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

        }
    }
}
