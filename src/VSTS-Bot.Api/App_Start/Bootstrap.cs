// ———————————————————————————————
// <copyright file="Bootstrap.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides method(s) to bootstrap the dependency injection framework.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Provides method(s) to bootstrap the dependency injection framework.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Bootstrap
    {
        /// <summary>
        /// Builds a <see cref="IContainer"/> that has all the necessary types registered to run the application.
        /// </summary>
        /// <param name="isDebugging">Flag that indicates if the application is in debugging modus.</param>
        /// <returns>A <see cref="IContainer"/>.</returns>
        public static IContainer Build(bool isDebugging)
        {
            Conversation.UpdateContainer(builder => Build(builder, isDebugging));

            return Conversation.Container;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Bootstrapper for Autofac. So it is intented to hit all needed dependencies in one place.")]
        private static void Build(ContainerBuilder builder, bool isDebugging)
        {
            builder
                .RegisterType<DialogInvoker>()
                .As<IDialogInvoker>();

            builder
                .RegisterModule<AttributedMetadataModule>();

            // Using a Telemetry Client per request, so user context, etc is unique per request.
            builder
                .RegisterType<TelemetryClient>()
                .SingleInstance();

            var microsoftAppCredentials =
                new MicrosoftAppCredentials(Config.MicrosoftApplicationId, Config.MicrosoftAppPassword);

            var client = new DocumentClient(Config.DocumentDbUri, Config.DocumentDbKey);
            IBotDataStore<BotData> store = new DocumentDbBotDataStore(client);

            builder
                .Register(c => client)
                .As<IDocumentClient>()
                .SingleInstance();

            builder
                .Register(c => store)
                .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c =>
                    new CachingBotDataStore(
                        store,
                        CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                .As<IBotDataStore<BotData>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            // When debugging with the bot emulator we need to use the listening url from the emulator.
            if (isDebugging && !string.IsNullOrEmpty(Config.EmulatorListeningUrl))
            {
                builder.Register(c => new StateClient(new Uri(Config.EmulatorListeningUrl), microsoftAppCredentials));
            }
            else
            {
                builder.Register(c => new StateClient(microsoftAppCredentials));
            }

            builder
                .RegisterType<BotState>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<BotDataFactory>()
                .As<IBotDataFactory>();

            builder
                .RegisterType<AuthenticationService>()
                .WithParameter("appSecret", Config.ApplicationSecret)
                .WithParameter("authorizeUrl", Config.AuthorizeUrl)
                .As<IAuthenticationService>();

            builder
                .RegisterType<VstsService>()
                .As<IVstsService>();

            builder
                .RegisterControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterApiControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IDialog<object>))))
                .Except<ConnectDialog>()
                .Except<RootDialog>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", Config.ApplicationId)
                .WithParameter("appScope", Config.ApplicationScope)
                .WithParameter("authorizeUrl", Config.AuthorizeUrl)
                .AsImplementedInterfaces();

            builder
                .RegisterType<RootDialog>()
                .AsSelf();
        }
    }
}