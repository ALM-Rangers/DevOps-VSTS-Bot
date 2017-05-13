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
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using DI;
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
        /// <param name="builder">Container builder to be used.</param>
        /// <param name="configurationProvider">The provider used to access configuration information.</param>
        /// <param name="isDebugging">Flag that indicates if the application is in debugging modus.</param>
        /// <returns>A <see cref="IContainer"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Bootstrapper for Autofac. So it is intented to hit all needed dependencies in one place.")]
        public static IContainer Build(ContainerBuilder builder, IConfigurationProvider configurationProvider, bool isDebugging)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            builder
                .RegisterInstance(configurationProvider);

            var microsoftAppCredentials = new MicrosoftAppCredentials(
                configurationProvider.GetValue(ConfigurationSettingName.MicrosoftApplicationId),
                configurationProvider.GetValue(ConfigurationSettingName.MicrosoftApplicationPassword));

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
                    new Uri(configurationProvider.GetValue(ConfigurationSettingName.EmulatorListeningUrl)), microsoftAppCredentials));
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
                .WithParameter("appSecret", configurationProvider.GetValue(ConfigurationSettingName.ApplicationSecret))
                .WithParameter("authorizeUrl", new Uri(configurationProvider.GetValue(ConfigurationSettingName.AuthorizeUrl)))
                .AsImplementedInterfaces();

            builder
                .RegisterType<BotService>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<VstsService>()
                .AsImplementedInterfaces();

            builder
                .RegisterControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterApiControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterType<DialogContextWrapper>()
                .As<IDialogContextWrapper>();

            builder
                .RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IDialog<object>))))
                .Except<ConnectDialog>()
                .Except<RootDialog>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", configurationProvider.GetValue(ConfigurationSettingName.ApplicationId))
                .WithParameter("authorizeUrl", new Uri(configurationProvider.GetValue(ConfigurationSettingName.AuthorizeUrl)))
                .AsImplementedInterfaces();

            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            return builder.Build();
        }
    }
}