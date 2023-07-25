using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class IntegerValue : NumericValue<long>
    {
        private long _value;
        public override long Value
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

        protected override int? maxDecimalPlaces => 0;

        public override JToken AsJToken()
        {
            if (Minimum.HasValue && Value < Minimum.Value)
                return Minimum.Value;
            if (Maximum.HasValue && Value > Maximum.Value)
                return Maximum.Value;
            return Value;
        }
    }
}
