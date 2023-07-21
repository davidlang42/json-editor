using Newtonsoft.Json;
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
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public StringProperty(string key, bool required) : base(key, required) { }

        public override string? ToJsonAssignment() => Value == null ? null : $"\"{Key}\": \"{JsonConvert.ToString(Value)}\"";

        public override IView GenerateView()
        {
            var control = new Entry
            {
                BindingContext = this
            };
            control.SetBinding(Entry.TextProperty, new Binding(nameof(Value), BindingMode.TwoWay));
            return control;
        }
    }
}
