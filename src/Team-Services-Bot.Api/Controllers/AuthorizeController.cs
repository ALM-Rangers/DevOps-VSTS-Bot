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
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.ApplicationInsights;
    using Resources;

    /// <summary>
    /// Handles the response from the oauth action.
    /// </summary>
    public class AuthorizeController : Controller
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IBotService botService;
        private readonly IProfileService profileService;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeController"/> class.
        /// </summary>
        /// <param name="telemetryClient">The TelemetryClient.</param>
        /// <param name="botService">The botService.</param>
        /// <param name="profileService">The profileService.s</param>
        /// <param name="authenticationService">The authenticationService.</param>
        public AuthorizeController(IBotService botService, TelemetryClient telemetryClient, IAuthenticationService authenticationService, IProfileService profileService)
        {
            this.authenticationService = authenticationService;
            this.botService = botService;
            this.profileService = profileService;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Handles the response for the oauth action.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="error">A error.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException">Occurs when code, error or state parameters are empty</exception>
        /// <returns>A view</returns>
        public async Task<ActionResult> Index(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var stateArray = state.Split(';');

            if (stateArray.Length != 2)
            {
                throw new ArgumentException(Exceptions.InvalidState, nameof(state));
            }

            var channelId = stateArray[0];
            var userId = stateArray[1];

            try
            {
                // Get the security token.
                var token = await this.authenticationService.GetToken(code);
                var profile = await this.profileService.GetProfile(token);
                var accounts = await this.profileService.GetAccounts(token, profile.Id);
                var result = new VstsProfile
                {
                    Accounts = accounts.Select(a => a.AccountName).ToList(),
                    Id = profile.Id,
                    Token = token
                };

                var data = await this.botService.GetUserData(channelId, userId);

                data.SetProperty("Profile", result);

                await this.botService.SetUserData(channelId, userId, data);
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                throw new Exception(Exceptions.UnknownException, ex);
            }

            return this.View();
        }
    }
}