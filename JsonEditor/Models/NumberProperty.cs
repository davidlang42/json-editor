using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class NumberProperty : Property
    {
        public long? Value { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }

        public NumberProperty(string key) : base(key) { }

        public override string? ToJsonAssignment() => Value == null ? null : $"\"{Key}\": {Value}";

        public override IView GenerateView()
        {
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(50, GridUnitType.Absolute)),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star)
                }
            };
            var entry = new Entry
            {
                BindingContext = this,
                Keyboard = Keyboard.Numeric
            };
            entry.SetBinding(Entry.TextProperty, new Binding(nameof(Value), BindingMode.TwoWay));
            grid.Add(entry);
            var stepper = new Stepper
            {
                BindingContext = this,
                Increment = 1
            };
            if (Minimum.HasValue)
                stepper.Minimum = Minimum.Value;
            if (Maximum.HasValue)
                stepper.Maximum = Maximum.Value;
            stepper.SetBinding(Stepper.ValueProperty, new Binding(nameof(Value), BindingMode.TwoWay));
            grid.Add(stepper);
            grid.SetColumn(stepper, 1);
            if (Minimum.HasValue && Maximum.HasValue)
            {
                var slider = new Slider
                {
                    Minimum = Minimum.Value,
                    Maximum = Maximum.Value
                };
                slider.SetBinding(Slider.ValueProperty, new Binding(nameof(Value), BindingMode.TwoWay));
                grid.Add(slider);
                grid.SetColumn(slider, 2);
            }
            return grid;
        }
    }
}
