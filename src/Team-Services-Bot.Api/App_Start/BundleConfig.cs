// ———————————————————————————————
// <copyright file="BundleConfig.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Controls the way we bundle css and javascript.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Optimization;

    /// <summary>
    /// Controls the way we bundle css and javascript.
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Bundles CSS and Javascript.
        /// </summary>
        public static void RegisterBundles() // BundleCollection bundles)
        {
            /*
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            */
        }
    }
}
