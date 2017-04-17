//———————————————————————————————
// <copyright file=”name of this file, i.e. Bootstrap.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Bootstraps Autofac.
// </summary>
//———————————————————————————————

using System.Linq;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Integration.WebApi;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Vsar.TSBot.Dialogs;

namespace Vsar.TSBot
{
    public static class Bootstrap
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

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