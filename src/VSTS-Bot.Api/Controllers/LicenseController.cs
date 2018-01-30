// ———————————————————————————————
// <copyright file="LicenseController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides the controller logic for the license pages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Mvc;

    /// <summary>
    /// Provides the controller logic for the license pages.
    /// </summary>
    public class LicenseController : Controller
    {
        /// <summary>
        /// Gets the license page.
        /// </summary>
        /// <returns>A view.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}