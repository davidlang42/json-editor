using CommunityToolkit.Maui.Behaviors;
using JsonEditor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class NumberValue : Value
    {
        private long _value;
        public long Value
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

        public override JToken AsJToken()
        {
            if (Minimum.HasValue && Value < Minimum.Value)
                return Minimum.Value;
            if (Maximum.HasValue && Value > Maximum.Value)
                return Maximum.Value;
            return Value;
        }

        public override string ToString()
        {
            var range_name = (Minimum, Maximum) switch
            {
                (double min, double max) => $" from {min} to {max}",
                (null, double max) => $" up to {max}",
                (double min, null) => $" from {min}",
                (null, null) => ""
            };
            return $"Number{range_name}";
        }

        public override View EditView
        {
            get
            {
                var grid = new Grid
                {
                    ColumnSpacing = 5,
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
                };
                entry.Behaviors.Add(NumericValidation(Minimum, Maximum));
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

        private static NumericValidationBehavior NumericValidation(double? minimum, double? maximum)
        {
            var behavior = new NumericValidationBehavior
            {
                InvalidStyle = InvalidStyle(),
                ValidStyle = ValidStyle(),
                Flags = ValidationFlags.ValidateOnValueChanged,
                MaximumDecimalPlaces = 0
            };

            if (minimum.HasValue)
                behavior.MinimumValue = minimum.Value;
            if (maximum.HasValue)
                behavior.MaximumValue = maximum.Value;

            return behavior;
        }
    }
}
