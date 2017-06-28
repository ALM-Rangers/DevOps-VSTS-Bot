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
    using System;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.DirectLine;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TechTalk.SpecFlow;

    public static class Config
    {
        static Config()
        {
            var x = TestContext.Properties["RefreshTokenReinitialize"];
            Console.WriteLine($"Config static constructor: {x}");

            RefreshTokenReinitialize = Convert.ToBoolean(x);
        }

        public static string Account => TestContext.Properties["Account"].ToString();

        public static ReleaseApproval Approval
        {
            get { return ScenarioContext.Current["Approval"] as ReleaseApproval; }
            set { ScenarioContext.Current["Approval"] = value; }
        }

        public static string AppSecret => TestContext.Properties["AppSecret"].ToString();

        public static Uri AuthorizeUrl => new Uri(TestContext.Properties["AuthorizeUrl"].ToString());

        public static string BotId => TestContext.Properties["BotId"].ToString();

        public static string BotSecret => TestContext.Properties["BotSecret"].ToString();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "As we are using Specflow we can not determine when the client is out of scope.")]
        public static IBotState BotState
        {
            get
            {
                if (!ScenarioContext.Current.ContainsKey("BotState"))
                {
                    var microsoftAppCredentials = new MicrosoftAppCredentials(
                        MicrosoftApplicationId,
                        MicrosoftApplicationPassword);

                    var client = new StateClient(microsoftAppCredentials);
                    ScenarioContext.Current["BotState"] = new BotState(client);
                }

                return ScenarioContext.Current["BotState"] as IBotState;
            }
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

        public static string MicrosoftApplicationId => TestContext.Properties["MicrosoftApplicationId"].ToString();

        public static string MicrosoftApplicationPassword => TestContext.Properties["MicrosoftApplicationPassword"].ToString();

        public static VstsProfile Profile
        {
            get { return (VstsProfile)ScenarioContext.Current["Profile"]; }
            set { ScenarioContext.Current["Profile"] = value; }
        }

        public static string RefreshToken => TestContext.Properties["RefreshToken"].ToString();

        public static bool RefreshTokenReinitialize { get; set; }

        public static string TeamProjectOne => TestContext.Properties["TeamProjectOne"].ToString();

        public static string TeamProjectTwo => TestContext.Properties["TeamProjectTwo"].ToString();

        public static OAuthToken Token
        {
            get { return (OAuthToken)ScenarioContext.Current["Token"]; }
            set { ScenarioContext.Current["Token"] = value; }
        }

        public static string UserName
        {
            get { return ScenarioContext.Current["UserName"].ToString(); }
            set { ScenarioContext.Current["UserName"] = value; }
        }

        private static TestContext TestContext => ScenarioContext.Current["TestContext"] as TestContext;
    }
}