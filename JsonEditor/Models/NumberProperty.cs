using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class NumberProperty : Property
    {
        public long Value { get; set; }

        public NumberProperty(string key) : base(key) { }

        public override string ToJson() => Value.ToString();

        public override IView GenerateView()
        {
            var control = new Entry
            {
                BindingContext = this,
                Keyboard = Keyboard.Numeric
            };
            control.SetBinding(Entry.TextProperty, new Binding(nameof(Value), BindingMode.TwoWay));
            return control;
        }
    }
}
