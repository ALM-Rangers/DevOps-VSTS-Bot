// ———————————————————————————————
// <copyright file="EventJsonConverter.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Converts json for an event to the correct derived.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Utils
{
    using System;
    using Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converts json for an event to the correct derived.
    /// </summary>
    public class EventJsonConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EventBase);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var @object = JObject.Load(reader);
            var type = @object["eventType"].ToString();

            switch (type)
            {
                case "ms.vss-release.deployment-approval-pending-event":
                    var target = new Event<ApprovalResource>();
                    serializer.Populate(@object.CreateReader(), target);
                    return target;
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}