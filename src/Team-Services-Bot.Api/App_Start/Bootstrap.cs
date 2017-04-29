// ———————————————————————————————
// <copyright file="Bootstrap.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Bootstraps Autofac.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Linq;
    using System.Web.Configuration;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Bootstraps Autofac.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Builds a <see cref="IContainer"/>.
        /// </summary>
        /// <param name="isDebugging">Flag that indicates if the application is in debugging modus.</param>
        /// <returns>A <see cref="IContainer"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
        public static IContainer Build(bool isDebugging)
        {
            var microsoftAppCredentials = new MicrosoftAppCredentials(
                WebConfigurationManager.AppSettings["MicrosoftAppId"],
                WebConfigurationManager.AppSettings["MicrosoftAppPassword"]);

            var builder = new ContainerBuilder();

            builder
                .RegisterModule<AttributedMetadataModule>();

            // Using a Telemetry Client per request, so user context, etc is unique per request.
            builder
                .RegisterType<TelemetryClient>()
                .SingleInstance();

            // When debugging with the bot emulator we need to use the listening url from the emulator.
            if (isDebugging)
            {
                builder.Register(c => new StateClient(
                    new Uri(WebConfigurationManager.AppSettings["EmulatorListeningUrl"]), microsoftAppCredentials));
            }
            else
            {
                builder.Register(c => new StateClient(microsoftAppCredentials));
            }

            builder
                .RegisterType<BotState>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<AuthenticationService>()
                .WithParameter("appSecret", WebConfigurationManager.AppSettings["AppSecret"])
                .WithParameter("authorizeUrl", new Uri(WebConfigurationManager.AppSettings["AuthorizeUrl"]))
                .AsImplementedInterfaces();

            builder
                .RegisterType<BotService>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ProfileService>()
                .AsImplementedInterfaces();

            builder
                .RegisterControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterApiControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IDialog<object>))))
                .Except<AccountConnectDialog>()
                .Except<RootDialog>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<AccountConnectDialog>()
                .WithParameter("appId", WebConfigurationManager.AppSettings["AppId"])
                .WithParameter("authorizeUrl", new Uri(WebConfigurationManager.AppSettings["AuthorizeUrl"]))
                .AsImplementedInterfaces();

            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            return builder.Build();
        }
    }
}