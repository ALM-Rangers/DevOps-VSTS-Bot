// ———————————————————————————————
// <copyright file="Config.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the general configuration settings.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using Microsoft.Bot.Connector.DirectLine;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TechTalk.SpecFlow;

    public static class Config
    {
        public static string BotId
        {
            get { return TestContext.Properties["BotId"].ToString(); }
            set { TestContext.Properties["BotId"] = value; }
        }

        public static string BotSecret
        {
            get { return TestContext.Properties["BotSecret"].ToString(); }
            set { TestContext.Properties["BotSecret"] = value; }
        }

        public static DirectLineClient Client
        {
            get { return ScenarioContext.Current["Client"] as DirectLineClient; }
            set { ScenarioContext.Current["Client"] = value; }
        }

        public static string ConversationId
        {
            get { return ScenarioContext.Current["ConversationId"].ToString(); }
            set { ScenarioContext.Current["ConversationId"] = value; }
        }

        private static TestContext TestContext => ScenarioContext.Current["TestContext"] as TestContext;
    }
}