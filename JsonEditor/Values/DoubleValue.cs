using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class DoubleValue : NumericValue<double>
    {
        private double _value;
        public override double Value
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

        /// <summary>Currently only supports factors of 10 (eg. 0.1, 0.01, etc)</summary>
        public double? MultipleOf { get; init; }

        protected override int? maxDecimalPlaces => MultipleOf.HasValue ? MeasureDecimalPlaces(MultipleOf.Value): null;

        public override JToken AsJToken()
        {
            if (Minimum.HasValue && Value < Minimum.Value)
                return Minimum.Value;
            if (Maximum.HasValue && Value > Maximum.Value)
                return Maximum.Value;
            return Value;
        }

        static int MeasureDecimalPlaces(double value)
        {
            value -= Math.Floor(value);
            value = Math.Abs(value);
            int dp = 0;
            while (value > 0 && value < 1)
            {
                value *= 10;
                dp++;
            }
            return dp;
        }
    }
}
