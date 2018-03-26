// ———————————————————————————————
// <copyright file="TermsController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides the controller logic for the terms pages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Represents the terms controller
    /// </summary>
    public class TermsController : Controller
    {
        /// <summary>
        /// Gets the terms page.
        /// </summary>
        /// <returns>A view.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}