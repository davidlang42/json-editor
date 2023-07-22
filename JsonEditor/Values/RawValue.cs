using CommunityToolkit.Maui.Behaviors;
using JsonEditor.Behaviors;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class RawValue : Value
    {
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        readonly JSchema schema;

        public RawValue(JToken? value, JSchema schema)
        {
            _value = value?.ToString(Formatting.Indented) ?? "";
            this.schema = schema;
        }

        public override JToken AsJToken() => JToken.Parse(Value);

        public override View EditView
        {
            get
            {
                var editor = new Editor
                {
                    BindingContext = this,
                    Placeholder = "Raw JSON"
                };
                editor.Behaviors.Add(JsonValidation(schema));
                editor.SetBinding(Editor.TextProperty, nameof(Value));
                return editor;
            }
        }

        private static JsonValidationBehavior JsonValidation(JSchema schema)
        {
            var behavior = new JsonValidationBehavior
            {
                InvalidStyle = InvalidStyle(),
                ValidStyle = ValidStyle(),
                Flags = ValidationFlags.ValidateOnValueChanged,
                Schema = schema
            };

            return behavior;
        }
    }
}
