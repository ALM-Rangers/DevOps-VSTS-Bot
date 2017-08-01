// ———————————————————————————————
// <copyright file="DefaultController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides the controller logic for the default pages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Mvc;

    /// <summary>
    /// Provides the controller logic for the default pages.
    /// </summary>
    public class DefaultController : Controller
    {
        /// <summary>
        /// Returns the default home page.
        /// </summary>
        /// <returns>A view.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}