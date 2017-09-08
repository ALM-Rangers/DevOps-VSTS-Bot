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
    using System.Web.Http;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Implements basic holding of <see cref="IVstsService"/> and <see cref="IVstsApplicationRegistry"/>.
    /// </summary>
    [Serializable]
    public abstract class DialogBase
    {
        [NonSerialized]
        private IVstsService vstsService;

        [NonSerialized]
        private IVstsApplicationRegistry vstsApplicationRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogBase"/> class.
        /// </summary>
        /// <param name="vstsService">VSTS accessor.</param>
        /// <param name="vstsApplicationRegistry">VSTS Application registry accessor.</param>
        protected DialogBase(IVstsService vstsService, IVstsApplicationRegistry vstsApplicationRegistry)
        {
            this.vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
            this.vstsApplicationRegistry = vstsApplicationRegistry ?? throw new ArgumentNullException(nameof(vstsApplicationRegistry));
        }

        /// <summary>
        /// Gets <see cref="IVstsService"/>
        /// </summary>
        protected IVstsService VstsService => this.vstsService;

        /// <summary>
        /// Gets <see cref="IVstsApplicationRegistry"/>.
        /// </summary>
        protected IVstsApplicationRegistry VstsApplicationRegistry => this.vstsApplicationRegistry;

        /// <summary>
        /// Gets <see cref="IAuthenticationService"/> for the <see cref="IMessageActivity"/>.
        /// </summary>
        /// <param name="activity">The <see cref="IMessageActivity"/>.</param>
        /// <returns><see cref="IAuthenticationService"/></returns>
        protected IAuthenticationService GetAuthenticationService(IMessageActivity activity)
        {
            return this.VstsApplicationRegistry
                .GetVstsApplicationRegistration(new VstsApplicationRegistrationKey(activity)).AuthenticationService;
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
            this.vstsApplicationRegistry = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsApplicationRegistry>();
        }
    }
}