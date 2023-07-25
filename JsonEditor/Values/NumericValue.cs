using CommunityToolkit.Maui.Behaviors;
using JsonEditor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal abstract class NumericValue<T> : Value where T : INumber<T>
    {
        public abstract T Value { get; set; }

        public double? Minimum { get; init; }
        public double? Maximum { get; init; }

        protected abstract int? maxDecimalPlaces { get; }

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
                entry.Behaviors.Add(NumericValidation(Minimum, Maximum, maxDecimalPlaces));
                entry.SetBinding(Entry.TextProperty, new Binding(nameof(Value), BindingMode.TwoWay));
                grid.Add(entry);
                var stepper = new Stepper
                {
                    BindingContext = this,
                    Increment = maxDecimalPlaces.HasValue ? Math.Pow(10, -maxDecimalPlaces.Value) : 1,
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

        private static NumericValidationBehavior NumericValidation(double? minimum, double? maximum, int? max_decimal_places)
        {
            var behavior = new NumericValidationBehavior
            {
                InvalidStyle = InvalidStyle(),
                ValidStyle = ValidStyle(),
                Flags = ValidationFlags.ValidateOnValueChanged
            };

            if (minimum.HasValue)
                behavior.MinimumValue = minimum.Value;
            if (maximum.HasValue)
                behavior.MaximumValue = maximum.Value;
            if (max_decimal_places.HasValue)
                behavior.MaximumDecimalPlaces = max_decimal_places.Value;

            return behavior;
        }
    }
}
