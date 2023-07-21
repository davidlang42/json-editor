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
        public string Value { get; set; } = "";

        public StringProperty(string key) : base(key) { }

        public override string ToJson() => $"\"{JsonConvert.ToString(Value)}\"";

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
