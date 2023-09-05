using JsonEditor.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonEditor.Converters
{
    internal class JsonPreview : IValueConverter
    {
        public Regex? ExcludeProperties { get; set; }
        public Regex? DontTraverseProperties { get; set; }
        public Regex? IncludeOnlyProperties { get; set; }
        public Regex? TraverseOnlyProperties { get; set; }
        public int MaximumSegements { get; set; } = 20;
        public string Separator { get; set; } = ", ";
        public string? FallbackValue { get; set; }
        public JSchema? Schema { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JObject obj)
            {
                var segments = ObjectPreview(obj, Schema).Distinct().Take(MaximumSegements).ToArray();
                if (segments.Length == 0)
                    return FallbackValue;
                return string.Join(Separator, segments);
            }
            else
            {
                return (value as JToken)?.ToString(Formatting.None).Truncate(400) ?? FallbackValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        bool MatchTraverseRules(string property)
        {
            if (DontTraverseProperties != null && DontTraverseProperties.IsMatch(property))
                return false;
            if (TraverseOnlyProperties == null)
                return true;
            return TraverseOnlyProperties.IsMatch(property);
        }

        bool MatchIncludeRules(string property)
        {
            if (ExcludeProperties != null && ExcludeProperties.IsMatch(property))
                return false;
            if (IncludeOnlyProperties == null)
                return true;
            return IncludeOnlyProperties.IsMatch(property);
        }

        IEnumerable<string> ObjectPreview(JObject obj, JSchema? schema, bool include_property_names = false)
        {
            foreach (var p in obj.Properties())
            {
                if (include_property_names)
                    yield return p.Name; // important for oneOf types so we know what type it is
                foreach (var s in PropertyPreview(p, schema))
                    yield return s;
            }
        }

        IEnumerable<string> PropertyPreview(JProperty prop, JSchema? object_schema)
            => ValuePreview(prop.Name, prop.Value, object_schema?.Properties.TryGetValue(prop.Name, out var s) == true ? s : null);

        IEnumerable<string> ArrayPreview(string name, JArray arr, JSchema? item_schema)
            => arr.Values<JToken>().SelectMany((v, i) => ValuePreview($"{name}[{i}]", v, item_schema));

        IEnumerable<string> ValuePreview(string name, JToken? value, JSchema? schema)
        {
            if (MatchTraverseRules(name))
            {
                switch (value?.Type)
                {
                    case JTokenType.Array when value is JArray arr:
                        foreach (var s in ArrayPreview(name, arr, schema?.Items.SingleOrDefaultSafe())) // only continue to traverse the schema if all array items are the same time, otherwise we'd have to figure out which one to use
                            yield return s;
                        break;
                    case JTokenType.Object when value is JObject obj:
                        foreach (var s in ObjectPreview(obj, schema, schema?.OneOf.Count > 0))
                            yield return s;
                        break;
                }
            }
            if (MatchIncludeRules(name))
            {
                switch (value?.Type)
                {
                    case JTokenType.Boolean when (bool)value:
                        yield return name;
                        break;
                    case JTokenType.Integer:
                    case JTokenType.Float:
                        yield return $"{name}={value}";
                        break;
                    case JTokenType.String:
                    case JTokenType.Date:
                    case JTokenType.Guid:
                    case JTokenType.Uri:
                    case JTokenType.TimeSpan:
                        yield return value.ToString().Trim();
                        break;
                };
            }
        }
    }
}
