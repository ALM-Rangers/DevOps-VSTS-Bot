// ———————————————————————————————
// <copyright file="AuthorizeController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Controller logic to handle the response from the oauth action.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// Handles the response from the oauth action.
    /// </summary>
    public class AuthorizeController : Controller
    {
        /// <summary>
        /// Handles the response for the oauth action.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="error">A error.</param>
        /// <param name="state">The state.</param>
        /// <returns>A view</returns>
        public async Task<ActionResult> Index(string code, string error, string state)
        {
            await Task.CompletedTask;

            return this.View();
        }
    }
}