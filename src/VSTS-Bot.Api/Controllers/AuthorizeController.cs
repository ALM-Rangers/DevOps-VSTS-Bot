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
        private readonly string appSecret;
        private readonly IAuthenticationService authenticationService;
        private readonly Uri authorizeUrl;
        private readonly IBotDataFactory botDataFactory;
        private readonly IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeController"/> class.
        /// </summary>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="authorizeUrl">The authorize url.</param>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="botDataFactory">The bot data factory;</param>
        /// <param name="vstsService">The profileService.s</param>
        public AuthorizeController(string appSecret, Uri authorizeUrl, IAuthenticationService authenticationService, IBotDataFactory botDataFactory, IVstsService vstsService)
        {
            appSecret.ThrowIfNull(nameof(appSecret));
            authorizeUrl.ThrowIfNull(nameof(authorizeUrl));

            this.appSecret = appSecret;
            this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            this.authorizeUrl = authorizeUrl;
            this.botDataFactory = botDataFactory ?? throw new ArgumentNullException(nameof(botDataFactory));
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
                var token = await this.authenticationService.GetToken(this.appSecret, this.authorizeUrl, code);
                var vstsProfile = await this.vstsService.GetProfile(token);
                var accounts = await this.vstsService.GetAccounts(token, vstsProfile.Id);
                var profile = CreateProfile(accounts, vstsProfile, token);

                var address = new Address(string.Empty, channelId, userId, string.Empty, string.Empty);
                var botData = this.botDataFactory.Create(address);
                await botData.LoadAsync(CancellationToken.None);

                var data = botData.UserData.GetValue<UserData>("userData");
                data.Profiles.Add(profile);

                botData.UserData.SetValue("userData", data);

                await botData.FlushAsync(CancellationToken.None);

                return this.View(new Authorize(data.Pin));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
            }

            return this.View(new Authorize(string.Empty));
        }

        private static Profile CreateProfile(IEnumerable<Account> accounts, Microsoft.VisualStudio.Services.Profile.Profile profile, OAuthToken token)
        {
            return new Profile
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