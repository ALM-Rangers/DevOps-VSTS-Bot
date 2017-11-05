// ———————————————————————————————
// <copyright file="DialogBase.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents base dialog that handles common services.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Implements basic holding of <see cref="IVstsService"/> and <see cref="IAuthenticationService"/>.
    /// </summary>
    [Serializable]
    public abstract class DialogBase
    {
        [NonSerialized]
        private IAuthenticationService authenticationService;

        [NonSerialized]
        private IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogBase"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="vstsService">VSTS accessor.</param>
        protected DialogBase(IAuthenticationService authenticationService, IVstsService vstsService)
        {
            this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            this.vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
        }

        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        protected IAuthenticationService AuthenticationService => this.authenticationService;

        /// <summary>
        /// Gets <see cref="IVstsService"/>
        /// </summary>
        protected IVstsService VstsService => this.vstsService;

        /// <summary>
        /// Validates the OAuthToken and refresh it if necessary.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <returns>A validated profile.</returns>
        protected async Task<Profile> GetValidatedProfile(IBotDataBag dataBag)
        {
            if (!dataBag.TryGetValue("userData", out UserData data))
            {
                return null;
            }

            var profile = data.Profile;

            if (profile != null && profile.Token.ExpiresOn.AddMinutes(-5) <= DateTime.UtcNow)
            {
                // Replace the current OAuth token.
                profile.Token = await this.authenticationService.GetToken(profile.Token);

                // Save it.
                dataBag.SetValue("userData", data);
            }

            return profile;
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.authenticationService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IAuthenticationService>();
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
        }
    }
}