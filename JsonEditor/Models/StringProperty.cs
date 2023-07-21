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
                    Value ??= "";
                else
                    Value = null;
            }
        }

        public StringProperty(JObject parent, string key, bool required) : base(parent, key, required) { }

        public override JToken? ValueAsJToken() => Value;

        public override IView GenerateEditView()
        {
            var entry = new Entry
            {
                BindingContext = this
            };
            entry.SetBinding(Entry.TextProperty, nameof(Value));
            if (Required)
            {
                Value ??= "";
                return entry;
            }
            else
            {
                entry.SetBinding(VisualElement.IsVisibleProperty, nameof(ValueIsNotNull));
                var checkbox = new CheckBox { BindingContext = this };
                checkbox.SetBinding(CheckBox.IsCheckedProperty, nameof(ValueIsNotNull));
                var label = new Label {
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
                grid.Add(checkbox);
                grid.Add(entry);
                grid.SetColumn(entry, 1);
                grid.Add(label);
                grid.SetColumn(label, 1);
                return grid;
            }
        }
    }
}
