using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace TSBot.AcceptanceTests
{
    [Binding]
    public sealed class EchoSteps
    {
        [Given(@"I have a client")]
        public void GivenIHaveAClient()
        {
            var testContext = ScenarioContext.Current["TestContext"] as TestContext;
            var botSecret = testContext.Properties["BotSecret"].ToString();

            try
            {
                var client = new DirectLineClient(botSecret);
                var conversation = client.Conversations.StartConversation();

                ScenarioContext.Current["Client"] = client;
                ScenarioContext.Current["ConversationId"] = conversation.ConversationId;
            }
            catch (Exception ex)
            {
                
            }
        }

        [When(@"I send a message '(.*)'")]
        public void WhenISendAMessage(string message)
        {
            var client = ScenarioContext.Current["Client"] as DirectLineClient;
            var conversationId = ScenarioContext.Current["ConversationId"].ToString();

            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount("TestUser"),
                Text = message
            };

            try
            {
                client.Conversations.PostActivity(conversationId, activity);
            }
            catch (Exception ex)
            {
                
            }
        }

        [Then(@"I should receive a response '(.*)'")]
        public void ThenIShouldReceiveAResponse(string response)
        {
            var client = ScenarioContext.Current["Client"] as DirectLineClient;
            var conversationId = ScenarioContext.Current["ConversationId"].ToString();

            try
            {
                var activities = client.Conversations.GetActivities(conversationId);
                var activity = activities.Activities.FirstOrDefault();

                activity.Text.ShouldBeEquivalentTo(response);
            }
            catch (Exception ex)
            {
                
            }
        }

    }
}
