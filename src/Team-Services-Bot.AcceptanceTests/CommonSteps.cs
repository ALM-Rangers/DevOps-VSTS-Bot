// ———————————————————————————————
// <copyright file="CommonSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the specflow steps to perform an echo.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using Microsoft.Bot.Connector.DirectLine;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Common steps.
    /// </summary>
    [Binding]
    public sealed class CommonSteps
    {
        /// <summary>
        /// Starts a conversation with the bot.
        /// </summary>
        [Given(@"I started a conversation")]
        public void GivenIStartedAConversation()
        {
            var client = new DirectLineClient(Config.BotSecret);
            var conversation = client.Conversations.StartConversation();

            Config.Client = client;
            Config.ConversationId = conversation.ConversationId;
        }
    }
}
