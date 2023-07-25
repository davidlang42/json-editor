using JsonEditor.Extensions;
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

        public static Value For(JsonModel.EditAction edit_object_action, JToken? value, JSchema schema)
        {
            return schema.Type switch
            {
                JSchemaType.String when schema.Enum.Count > 0 => new EnumStringValue
                {
                    Value = (value as JValue)?.Value as string ?? "",
                    ValidStrings = schema.Enum.Select(j => ((JValue)j).Value as string ?? throw new ApplicationException($"Invalid enum: {j}")).ToArray()
                },
                JSchemaType.String => new StringValue
                {
                    Value = (value as JValue)?.Value as string ?? "",
                    MinLength = (int?)schema.MinimumLength,
                    MaxLength = (int?)schema.MaximumLength,
                    Pattern = schema.Pattern
                },
                JSchemaType.Integer => new NumberValue
                {
                    Value = (value as JValue)?.Value as long? ?? 0,
                    Minimum = schema.Minimum,
                    Maximum = schema.Maximum
                },
                JSchemaType.Boolean => new BooleanValue
                {
                    Value = (value as JValue)?.Value as bool? ?? false
                },
                JSchemaType.Array when schema.Items.SingleOrDefaultSafe() is JSchema one_type_of_item
                    => new ArrayValue(edit_object_action, value as JArray ?? new JArray(), one_type_of_item, schema.MinimumItems, schema.MaximumItems),
                JSchemaType.Object => new ObjectValue(edit_object_action)
                {
                    Value = value as JObject ?? new JObject(),
                    ObjectSchema = schema,
                    ObjectType = schema.Title
                },
                null when schema.OneOf.Count > 0 => new OneOfValue(edit_object_action, value, schema.OneOf.ToArray()),
                _ => new RawValue(value, schema)
            };
        }
    }
}
