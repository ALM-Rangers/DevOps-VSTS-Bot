using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using Microsoft.Bot.Builder.Dialogs;

namespace Vsar.TSBot.DI
{
    public static class ComponentContextExtensions
    {
        public static IDialog<object> Find(this IComponentContext context, string activityText)
        {
            var dialogs = context.Resolve<IEnumerable<Meta<IDialog<object>>>>();

            return dialogs
                .Where(m => activityText.ToLower().StartsWith(m.Metadata["Command"].ToString().ToLower()))
                .Select(m => m.Value)
                .FirstOrDefault();
        }
    }
}