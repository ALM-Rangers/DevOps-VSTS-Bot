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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.Services.Account;
    using Resources;

    /// <summary>
    /// Provides method(s) so the user can authorize himself against VSTS.
    /// </summary>
    public class AuthorizeController : Controller
    {
        private readonly IVstsApplicationRegistry applicationRegistry;
        private readonly IBotDataFactory botDataFactory;
        private readonly IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeController"/> class.
        /// </summary>
        /// <param name="botDataFactory">The bot data factory;</param>
        /// <param name="applicationRegistry">VSTS Application Registry</param>
        /// <param name="vstsService">The profileService.s</param>
        public AuthorizeController(IBotDataFactory botDataFactory, IVstsApplicationRegistry applicationRegistry, IVstsService vstsService)
        {
            this.botDataFactory = botDataFactory ?? throw new ArgumentNullException(nameof(botDataFactory));
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
            var pin = string.Empty;

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
                var token = await this.applicationRegistry.GetVstsApplicationRegistration(userId).AuthenticationService.GetToken(code);
                var profile = await this.vstsService.GetProfile(token);
                var accounts = await this.vstsService.GetAccounts(token, profile.Id);
                var vstsProfile = CreateVstsProfile(accounts, profile, token);

                var address = new Address(string.Empty, channelId, userId, string.Empty, string.Empty);
                var botData = this.botDataFactory.Create(address);
                await botData.LoadAsync(CancellationToken.None);

                pin = botData.UserData.GetPin();

                botData.UserData.SetNotValidatedByPinProfile(vstsProfile);

                await botData.FlushAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
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