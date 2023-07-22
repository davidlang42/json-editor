using JsonEditor.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class StringProperty : Property
    {
        private string? _value;
        public string? Value
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

        public StringProperty(JsonModel model, JObject parent, string key, bool required) : base(model, parent, key, required)
        {
            Value = parent.Value<string>(key);
        }

        public override JToken ValueAsJToken() => Value;

        public override VisualElement GenerateEditView()
        {
            var entry = new Entry
            {
                BindingContext = this
            };
            entry.SetBinding(Entry.TextProperty, nameof(Value));
            return entry;
        }
    }
}
