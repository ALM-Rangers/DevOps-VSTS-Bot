// ———————————————————————————————
// <copyright file="BuildsDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog to list/queue builds.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents the dialog to list/queue builds.
    /// </summary>
    [CommandMetadata("builds")]
    [Serializable]
    public class BuildsDialog : IDialog<object>
    {
        [NonSerialized]
        private IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildsDialog"/> class.
        /// </summary>
        /// <param name="vstsService">VSTS accessor</param>
        /// <exception cref="ArgumentNullException">Occurs when the vstsService is missing.</exception>
        public BuildsDialog(IVstsService vstsService)
        {
            if (vstsService == null)
            {
                throw new ArgumentNullException(nameof(vstsService));
            }

            this.vstsService = vstsService;
        }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.BuildsAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a list of builds.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task BuildsAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var activity = await result;
            var text = (activity.Text ?? string.Empty).ToLowerInvariant();
        }
    }
}