using CommunityToolkit.Maui.Behaviors;
using JsonEditor.Converters;
using JsonEditor.Extensions;
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
    internal class StringValue : Value
    {
        private string _value = "";
        public string Value
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

        /// <summary>NOTE: The regex pattern is for visual validation only, it will allow saving an invalid string.</summary>
        public string? Pattern { get; init; }
        public int? MinLength { get; init; }
        public int? MaxLength { get; init; }

        public override JToken AsJToken()
        {
            if (MinLength.HasValue && Value.Length < MinLength.Value)
                return Value.PadRight(MinLength.Value);
            if (MaxLength.HasValue && Value.Length > MaxLength.Value)
                return Value.Truncate(MaxLength.Value);
            return Value; // this allows saving a string which doesn't match the regex pattern, but we can't easily fix that
        }

        public override string ToString() => "String";

        public override View EditView
        {
            get
            {
                var entry = new Entry
                {
                    BindingContext = this,
                    MaxLength = MaxLength ?? int.MaxValue
                };
                entry.Behaviors.Add(TextValidation(MinLength, MaxLength, Pattern));
                entry.SetBinding(Entry.TextProperty, nameof(Value));
                return entry;
            }
        }

        private static TextValidationBehavior TextValidation(int? minLength, int? maxLength, string? pattern)
        {
            var behavior = new TextValidationBehavior
            {
                InvalidStyle = InvalidStyle(),
                ValidStyle = ValidStyle(),
                Flags = ValidationFlags.ValidateOnValueChanged,
            };

            if (minLength.HasValue)
                behavior.MinimumLength = minLength.Value;
            if (maxLength.HasValue)
                behavior.MaximumLength = maxLength.Value;
            if (pattern != null)
                behavior.RegexPattern = pattern;

            return behavior;
        }
    }
}
