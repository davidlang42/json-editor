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
        private bool? _value;
        public bool? Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    var was_null = _value is null;
                    _value = value;
                    NotifyPropertyChanged();
                    var is_null = _value is null;
                    if (was_null != is_null)
                        NotifyPropertyChanged(nameof(ValueIsNotNull));
                }
            }
        }

        public bool ValueIsNotNull
        {
            get => Value != null;
            set
            {
                if (value)
                    Value ??= false;
                else
                    Value = null;
            }
        }

        public BooleanProperty(JObject parent, string key, bool required) : base(parent, key, required) { }

        public override JToken? ValueAsJToken() => Value;

        public override IView GenerateEditView()
        {
            var checkbox = new CheckBox
            {
                BindingContext = this
            };
            checkbox.SetBinding(CheckBox.IsCheckedProperty, nameof(Value));
            if (Required)
            {
                ValueIsNotNull = true;
                return checkbox;
            }
            else
            {
                checkbox.SetBinding(VisualElement.IsVisibleProperty, nameof(ValueIsNotNull));
                var null_switch = new Switch
                {
                    BindingContext = this
                };
                null_switch.SetBinding(Switch.IsToggledProperty, nameof(ValueIsNotNull));
                var label = new Label
                {
                    BindingContext = this,
                    Text = "(null)",
                    FontAttributes = FontAttributes.Italic
                };
                label.SetBinding(VisualElement.IsVisibleProperty, nameof(ValueIsNotNull), converter: new InvertBoolean());
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star)
                    }
                };
                grid.Add(null_switch);
                grid.Add(checkbox);
                grid.SetColumn(checkbox, 1);
                grid.Add(label);
                grid.SetColumn(label, 1);
                return grid;
            }
        }
    }
}
