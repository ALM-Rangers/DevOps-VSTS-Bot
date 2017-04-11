using Microsoft.Bot.Connector.DirectLine;
using TechTalk.SpecFlow;

namespace TSBot.AcceptanceTests
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
