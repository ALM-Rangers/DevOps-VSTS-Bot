// ———————————————————————————————
// <copyright file="PrivacyController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides the controller logic for the privacy pages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Mvc;

    /// <summary>
    /// Provides the controller logic for the privacy pages.
    /// </summary>
    public class PrivacyController : Controller
    {
        /// <summary>
        /// Gets the privacy page.
        /// </summary>
        /// <returns>A view.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}