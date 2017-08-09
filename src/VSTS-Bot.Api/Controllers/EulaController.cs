// ———————————————————————————————
// <copyright file="EulaController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides the controller logic for the eula pages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Mvc;

    /// <summary>
    /// Provides the controller logic for the eula pages.
    /// </summary>
    public class EulaController : Controller
    {
        /// <summary>
        /// Gets the EULA page.
        /// </summary>
        /// <returns>A view.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}