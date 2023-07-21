using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public abstract class Property
    {
        public string Key { get; }
        public bool Required { get; set; }

        public Property(string key)
        {
            Key = key;
        }

        /// <summary>Convert this property's current value to a Json assignment, or null if the value is not set.</summary>
        public abstract string? ToJsonAssignment();

        public abstract IView GenerateView();

        public static Property For(string key, JSchema schema, JToken? current)
        {
            var value = (current as JValue)?.Value;
            Property property = schema.Type switch
            {
                JSchemaType.String => new StringProperty(key) {
                    Value = value as string ?? ""
                },
                JSchemaType.Integer => new NumberProperty(key) {
                    Value = value as long? ?? 0,
                    Minimum = ToNullableInt64(schema.Minimum),
                    Maximum = ToNullableInt64(schema.Maximum)
                },
                _ => new UnsupportedProperty(key, current?.ToString())
            };
            property.Required = schema.Required.Contains(key);
            return property;
        }

        static long? ToNullableInt64(double? number_to_round) => number_to_round == null ? null : Convert.ToInt64(number_to_round);
    }
}
