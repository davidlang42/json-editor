using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public RawValue(JToken? value)
        {
            _value = value?.ToString(Formatting.Indented) ?? "";
        }

        public override JToken AsJToken() => JToken.Parse(Value);

        public override View EditView => new Label
        {
            Text = $"Unsupported type: {Value.Replace(Environment.NewLine, "").Truncate(400)}", //TODO make this editable as text, but be aware it will be ultra slow for large sections
            LineBreakMode = LineBreakMode.NoWrap
        };
    }
}
