using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public abstract class Property : INotifyPropertyChanged
    {
        public string Key { get; }
        public bool Required { get; }

        public Property(string key, bool required)
        {
            Key = key;
            Required = required;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Convert this property's current value to a Json assignment, or null if the value is not set.</summary>
        public abstract string? ToJsonAssignment();

        public abstract IView GenerateView();

        public static Property For(string key, JSchema schema, JToken? current)
        {
            var value = (current as JValue)?.Value;
            var required = schema.Required.Contains(key);
            Property property = schema.Type switch
            {
                JSchemaType.String => new StringProperty(key, required) {
                    Value = value as string ?? ""
                },
                JSchemaType.Integer => new NumberProperty(key, required) {
                    Value = value as long? ?? 0,
                    Minimum = ToNullableInt64(schema.Minimum),
                    Maximum = ToNullableInt64(schema.Maximum)
                },
                _ => new UnsupportedProperty(key, required, current?.ToString())
            };
            return property;
        }

        static long? ToNullableInt64(double? number_to_round) => number_to_round == null ? null : Convert.ToInt64(number_to_round);
    }
}
