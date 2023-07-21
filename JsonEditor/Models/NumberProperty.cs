using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class NumberProperty : Property
    {
        private long? _value;
        public long? Value
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

        public double? Minimum { get; init; }
        public double? Maximum { get; init; }

        public NumberProperty(JObject parent, string key, bool required) : base(parent, key, required) { }

        public override JToken? ValueAsJToken() => Required ? (Value ?? 0) : Value;

        public override IView GenerateEditView()
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
                Keyboard = Keyboard.Numeric,
                Placeholder = Required ? "0" : "(null)"
            };
            //TODO implement validation on text entry: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/behaviors/numeric-validation-behavior
            entry.SetBinding(Entry.TextProperty, new Binding(nameof(Value), BindingMode.TwoWay));
            grid.Add(entry);
            var stepper = new Stepper
            {
                BindingContext = this,
                Increment = 1,
                Minimum = Minimum ?? long.MinValue,
                Maximum = Maximum ?? long.MaxValue,
            };
            stepper.SetBinding(Stepper.ValueProperty, new Binding(nameof(Value), BindingMode.TwoWay));
            grid.Add(stepper);
            grid.SetColumn(stepper, 1);
            if (Minimum.HasValue && Maximum.HasValue)
            {
                var slider = new Slider
                {
                    BindingContext = this,
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
