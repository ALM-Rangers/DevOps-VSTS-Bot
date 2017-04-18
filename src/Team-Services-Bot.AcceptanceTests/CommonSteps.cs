//———————————————————————————————
// <copyright file=”name of this file, i.e. CommonSteps.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the common spec flow steps.
// </summary>
//———————————————————————————————

using Microsoft.Bot.Connector.DirectLine;
using TechTalk.SpecFlow;

namespace Vsar.TSBot.AcceptanceTests
{
    [Binding]
    public sealed class CommonSteps
    {
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
