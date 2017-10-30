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
    using System.Globalization;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.DirectLine;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TechTalk.SpecFlow;

    public static class Config
    {
        private static readonly object PadLock = new object();
        private static bool? refreshTokenReinitialize;
        private static IBotDataStore<BotData> store;

        public static string Account => TestContext.Properties["Account"].ToString();

        public static ReleaseApproval Approval
        {
            get => ScenarioContext.Current["Approval"] as ReleaseApproval;
            set => ScenarioContext.Current["Approval"] = value;
        }

        public static string AppSecret => TestContext.Properties["AppSecret"].ToString();

        public static Uri AuthorizeUrl => new Uri(TestContext.Properties["AuthorizeUrl"].ToString());

        public static string BotId => TestContext.Properties["BotId"].ToString();

        public static string BotSecret => TestContext.Properties["BotSecret"].ToString();

        public static int BuildId
        {
            get => Convert.ToInt32(ScenarioContext.Current["BuildId"], CultureInfo.InvariantCulture);
            set => ScenarioContext.Current["BuildId"] = value;
        }

        public static DirectLineClient Client
        {
            get => ScenarioContext.Current["Client"] as DirectLineClient;
            set => ScenarioContext.Current["Client"] = value;
        }

        public static string ConversationId
        {
            get => ScenarioContext.Current["ConversationId"].ToString();
            set => ScenarioContext.Current["ConversationId"] = value;
        }

        public static string DocumentDbKey => TestContext.Properties["DocumentDbKey"].ToString();

        public static Uri DocumentDbUri => new Uri(TestContext.Properties["DocumentDbUri"].ToString());

        public static string MicrosoftApplicationId => TestContext.Properties["MicrosoftApplicationId"].ToString();

        public static string MicrosoftApplicationPassword => TestContext.Properties["MicrosoftApplicationPassword"].ToString();

        public static VstsProfile Profile
        {
            get => (VstsProfile)ScenarioContext.Current["Profile"];
            set => ScenarioContext.Current["Profile"] = value;
        }

        public static int ReleaseId
        {
            get => Convert.ToInt32(ScenarioContext.Current["ReleaseId"], CultureInfo.InvariantCulture);
            set => ScenarioContext.Current["ReleaseId"] = value;
        }

        public static string RefreshToken => TestContext.Properties["RefreshToken"].ToString();

        public static bool RefreshTokenReinitialize
        {
            get => refreshTokenReinitialize.GetValueOrDefault(Convert.ToBoolean(TestContext.Properties["RefreshTokenReinitialize"], CultureInfo.InvariantCulture));
            set => refreshTokenReinitialize = value;
        }

        public static IBotDataStore<BotData> Store
        {
            get
            {
                lock (PadLock)
                {
                    if (store == null)
                    {
                        var client = new DocumentClient(DocumentDbUri, DocumentDbKey);
                        store = new CachingBotDataStore(new DocumentDbBotDataStore(client), CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency);
                    }
                }

                return store;
            }
        }

        public static string TeamProjectOne => TestContext.Properties["TeamProjectOne"].ToString();

        public static string TeamProjectTwo => TestContext.Properties["TeamProjectTwo"].ToString();

        public static TestContext TestContext => ScenarioContext.Current["TestContext"] as TestContext;

        public static OAuthToken Token
        {
            get => (OAuthToken)ScenarioContext.Current["Token"];
            set => ScenarioContext.Current["Token"] = value;
        }

        public static string UserName
        {
            get => ScenarioContext.Current["UserName"].ToString();
            set => ScenarioContext.Current["UserName"] = value;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test code, nothing to worry about.")]
        public static IBotData GetBotData()
        {
            var address = new Address(string.Empty, ChannelIds.Directline, UserName, string.Empty, string.Empty);
            return new JObjectBotData(address, Store);
        }
    }
}