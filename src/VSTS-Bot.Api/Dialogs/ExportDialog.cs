// ———————————————————————————————
// <copyright file="ExportDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog for Exporting user data to the user.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the dialog for Exporting to a VSTS account and team project.
    /// </summary>
    [CommandMetadata("export")]
    [Serializable]
    public class ExportDialog : DialogBase, IDialog<object>
    {
        private const string CommandMatchExport = @"export";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        public ExportDialog(IAuthenticationService authenticationService, IVstsService vstsService)
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

            context.Wait(this.ExportAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a list of release definitions.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task ExportAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            var text = (activity.Text ?? string.Empty).Trim().ToLowerInvariant();

            if (text.Equals(CommandMatchExport, StringComparison.OrdinalIgnoreCase))
            {
                if (activity.Conversation.IsGroup.HasValue && activity.Conversation.IsGroup.Value)
                {
                    var reply = context.MakeMessage();
                    reply.Text = string.Format("This is a group chat, data cannot be exported.");
                    await context.PostAsync(reply);
                    context.Done(reply);
                }
                else
                {
                    var data = context.UserData.GetValue<UserData>("userData");
                    var userProfileId = data.Profile.Id;
                    if (!string.IsNullOrEmpty(userProfileId.ToString()))
                    {
                        var reply = context.MakeMessage();
                        reply.Text = string.Format("ProfileId: {0}", userProfileId);
                        await context.PostAsync(reply);
                        context.Done(reply);
                    }
                    else
                    {
                        context.Fail(new UnknownCommandException(activity.Text));
                    }
                }
            }
            else
            {
                context.Fail(new UnknownCommandException(activity.Text));
            }
        }
    }
}