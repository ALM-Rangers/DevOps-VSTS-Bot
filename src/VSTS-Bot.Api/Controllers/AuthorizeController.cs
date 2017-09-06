// ———————————————————————————————
// <copyright file="AuthorizeController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides method(s) so the user can authorize himself against VSTS.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.Services.Account;
    using Resources;

    /// <summary>
    /// Provides method(s) so the user can authorize himself against VSTS.
    /// </summary>
    public class AuthorizeController : Controller
    {
        private readonly IVstsApplicationRegistry applicationRegistry;
        private readonly IBotService botService;
        private readonly IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeController"/> class.
        /// </summary>
        /// <param name="botService">The botService.</param>
        /// <param name="applicationRegistry">VSTS Application Registry</param>
        /// <param name="vstsService">The profileService.s</param>
        public AuthorizeController(IBotService botService, IVstsApplicationRegistry applicationRegistry, IVstsService vstsService)
        {
            this.botService = botService ?? throw new ArgumentNullException(nameof(botService));
            this.applicationRegistry = applicationRegistry ?? throw new ArgumentNullException(nameof(applicationRegistry));
            this.vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
        }

        /// <summary>
        /// Handles the response for the oauth action for VSTS.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="error">A error.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException">Occurs when code, error or state parameters are empty</exception>
        /// <returns>A view</returns>
        public async Task<ActionResult> Index(string code, string error, string state)
        {
            var stateArray = (state ?? string.Empty).Split(';');
            string pin = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(error))
                {
                    throw new ArgumentNullException(nameof(code));
                }

                if (stateArray.Length != 2)
                {
                    throw new ArgumentException(Exceptions.InvalidState, nameof(state));
                }

                var channelId = stateArray[0];
                var userId = stateArray[1];

                // Get the security token.
                var sessionKey = new VstsApplicationRegistrationKey(channelId, userId);
                var token = await this.applicationRegistry.GetVstsApplicationRegistration(sessionKey).AuthenticationService.GetToken(code);
                var profile = await this.vstsService.GetProfile(token);
                var accounts = await this.vstsService.GetAccounts(token, profile.Id);
                var vstsProfile = CreateVstsProfile(accounts, profile, token);

                var data = await this.botService.GetUserData(channelId, userId);
                pin = data.GetPin();

                data.SetNotValidatedByPinProfile(vstsProfile);

                await this.botService.SetUserData(channelId, userId, data);
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError(string.Empty, e.Message);
            }

            return this.View(new Authorize(pin));
        }

        private static VstsProfile CreateVstsProfile(IEnumerable<Account> accounts, Microsoft.VisualStudio.Services.Profile.Profile profile, OAuthToken token)
        {
            return new VstsProfile
            {
                Accounts = accounts.Select(a => a.AccountName).ToList(),
                DisplayName = profile.DisplayName,
                EmailAddress = profile.EmailAddress,
                Id = profile.Id,
                Token = token
            };
        }
    }
}