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
    internal class BooleanProperty : Property
    {
        private bool _value;
        public bool Value
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

        public BooleanProperty(JsonModel model, JObject parent, string key, bool required) : base(model, parent, key, required)
        {
            Value = parent.Value<bool>(key);
        }

        public override JToken ValueAsJToken() => Value;

        public override VisualElement GenerateEditView()
        {
            var checkbox = new CheckBox
            {
                BindingContext = this
            };
            checkbox.SetBinding(CheckBox.IsCheckedProperty, nameof(Value));
            return checkbox;
        }
    }
}
