﻿using JsonEditor.Extensions;
using JsonEditor.Models;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    public abstract class Value : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract View EditView { get; }

        public abstract JToken AsJToken();

        public override abstract string ToString();

        protected static Style ValidStyle()
        {
            var valid = new Style(typeof(VisualElement));
            valid.Setters.Add(new Setter
            {
                Property = VisualElement.BackgroundColorProperty,
                Value = Colors.Transparent
            });
            return valid;
        }

        protected static Style InvalidStyle()
        {
            var invalid = new Style(typeof(VisualElement));
            invalid.Setters.Add(new Setter
            {
                Property = VisualElement.BackgroundColorProperty,
                Value = Colors.LightCoral
            });
            return invalid;
        }

        /// <summary>Recursively list all Values nested under (and including) this one.</summary>
        public virtual IEnumerable<Value> Recurse() => this.Yield();

        public static Value For(JsonModel.EditAction edit_object_action, Regex? hide_properties, Regex? name_properties, JToken? value, JSchema schema)
        {
            return schema.Type switch
            {
                JSchemaType.String when schema.Enum.Count > 0 => new EnumStringValue
                {
                    Value = (value as JValue)?.Value as string ?? (schema.Default?.DeepClone() as JValue)?.Value as string ?? "",
                    ValidStrings = schema.Enum.Select(j => ((JValue)j).Value as string ?? throw new ApplicationException($"Invalid enum: {j}")).ToArray()
                },
                JSchemaType.String => new StringValue
                {
                    Value = (value as JValue)?.Value as string ?? (schema.Default?.DeepClone() as JValue)?.Value as string ?? "",
                    MinLength = (int?)schema.MinimumLength,
                    MaxLength = (int?)schema.MaximumLength,
                    Pattern = schema.Pattern
                },
                JSchemaType.Integer => new IntegerValue
                {
                    Value = (value as JValue)?.Value as long? ?? (schema.Default?.DeepClone() as JValue)?.Value as long? ?? 0,
                    Minimum = schema.Minimum,
                    Maximum = schema.Maximum
                },
                JSchemaType.Number => new DoubleValue
                {
                    Value = (value as JValue)?.Value as double? ?? (schema.Default?.DeepClone() as JValue)?.Value as double? ?? 0,
                    Minimum = schema.Minimum,
                    Maximum = schema.Maximum,
                    MultipleOf = schema.MultipleOf
                },
                JSchemaType.Boolean => new BooleanValue
                {
                    Value = (value as JValue)?.Value as bool? ?? (schema.Default?.DeepClone() as JValue)?.Value as bool? ?? false
                },
                JSchemaType.Array when schema.Items.SingleOrDefaultSafe() is JSchema one_type_of_item
                    => new ArrayValue(edit_object_action, hide_properties, name_properties, value as JArray ?? (schema.Default?.DeepClone() as JArray) ?? new JArray(), one_type_of_item, schema.MinimumItems, schema.MaximumItems),
                JSchemaType.Object => new ObjectValue(edit_object_action, hide_properties, name_properties)
                {
                    Value = value as JObject ?? schema.Default?.DeepClone() as JObject ?? new JObject(),
                    ObjectSchema = schema,
                    ObjectType = schema.Title
                },
                null when schema.OneOf.Count > 0 => new OneOfValue(edit_object_action, hide_properties, name_properties, value ?? schema.Default?.DeepClone(), schema.OneOf.ToArray()),
                _ => new RawValue(value ?? schema.Default?.DeepClone(), schema)
            };
        }
    }
}
