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
    using Autofac.Integration.WebApi;
    using DI;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Bootstraps Autofac.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Builds a <see cref="IContainer"/>.
        /// </summary>
        /// <param name="builder">Container builder to be used.</param>
        /// <returns>A <see cref="IContainer"/>.</returns>
        public static IContainer Build(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .RegisterModule<AttributedMetadataModule>();

            // Using a Telemetry Client per request, so user context, etc is unique per request.
            builder
                .RegisterType<TelemetryClient>()
                .InstancePerRequest();

            builder
                .RegisterApiControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IDialog<object>))))
                .Except<RootDialog>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            return builder.Build();
        }
    }
}