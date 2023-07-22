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

        public override View EditView
        {
            get
            {
                var editor = new Editor
                {
                    BindingContext = this,
                    Placeholder = "Raw json"
                };
                //TODO validation for json
                editor.SetBinding(Editor.TextProperty, nameof(Value));
                return editor;
            }
        } 
    }
}
