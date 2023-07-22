using JsonEditor.Converters;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class BooleanValue : Value
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

        public override JToken AsJToken() => Value;

        public override VisualElement EditView
        {
            get
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
}
