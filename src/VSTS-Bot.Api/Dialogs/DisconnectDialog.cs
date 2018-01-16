// ———————————————————————————————
// <copyright file="DisconnectDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog for Disconnecting to a VSTS account and team project.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the dialog for disconnecting to a VSTS account and team project.
    /// </summary>
    [CommandMetadata("Disconnect")]
    [Serializable]
    public class DisconnectDialog : DialogBase, IDialog<object>
    {
        private const string CommandMatchDisConnect = @"disconnect";

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        public DisconnectDialog(IAuthenticationService authenticationService, IVstsService vstsService)
            : base(authenticationService, vstsService)
        {
        }

        /// <summary>
        /// Gets or sets an account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets a pin.
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="TSBot.Profile"/>.
        /// </summary>
        public Profile Profile { get; set; }

        /// <summary>
        /// Gets or sets a collection of <see cref="TSBot.Profile"/>.
        /// </summary>
        public IEnumerable<Profile> Profiles { get; set; }

        /// <summary>
        /// Gets or sets a team project.
        /// </summary>
        public string TeamProject { get; set; }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.ThrowIfNull(nameof(context));

            context.Wait(this.DisconnectAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a list of release definitions.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task DisconnectAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            var data = context.UserData.GetValue<UserData>("userData");

            this.Account = data.Account;
            this.Profile = await this.GetValidatedProfile(context.UserData);
            this.TeamProject = data.TeamProject;

            var text = (activity.Text ?? string.Empty).Trim().ToLowerInvariant();

            if (text.Equals(CommandMatchDisConnect, StringComparison.OrdinalIgnoreCase))
            {
                var reply = context.MakeMessage();
                reply.Text = Labels.DisConnected;
                await context.PostAsync(reply);
                context.Done(reply);
            }
            else
            {
                context.Fail(new UnknownCommandException(Labels.Connect));
            }
        }
    }
}