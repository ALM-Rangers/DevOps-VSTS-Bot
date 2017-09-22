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

        /// <summary>
        /// Returns how it works page.
        /// </summary>
        /// <param name="channels">The channels to show</param>
        /// <returns>A view.</returns>
        public ActionResult HowItWorks(Channel channels = Channel.All)
        {
            return this.View(new { Channels = channels });
        }
    }
}