using JsonEditor.Converters;
using JsonEditor.Extensions;
using Microsoft.Extensions.Options;
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

namespace JsonEditor.Models
{
    public abstract class Property : INotifyPropertyChanged
    {
        public JsonModel Model { get; }
        public string Key { get; }
        public bool Required { get; }

        private bool _include = true;
        public bool Include
        {
            get => _include;
            set
            {
                if (value != _include)
                {
                    _include = value;
                    NotifyPropertyChanged();
                }
            }
        }

        readonly JObject parent;

        public Property(JsonModel model, JObject parent, string key, bool required)
        {
            Model = model;
            this.parent = parent;
            Key = key;
            Required = required;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Commit()
        {
            if (Include)
                parent[Key] = ValueAsJToken();
            else
                parent.Remove(Key);
        }

        public abstract VisualElement GenerateEditView();

        public abstract JToken ValueAsJToken();

        public IView GenerateHeaderView()
        {
            //TODO right click property header > copy, paste (json as text)
            return new Label
            {
                Text = Key,
                FontAttributes = Required ? FontAttributes.Bold : FontAttributes.None
            };
        }

        protected static string PreviewJson(JToken value) => value.ToString(Formatting.None).Truncate(400);

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

        public static Property For(JsonModel model, JObject parent, string key, JSchema schema, bool required)
        {
            Property property = schema.Type switch
            {
                JSchemaType.String when schema.Enum.Count > 0 => new EnumStringProperty(model, parent, key, required) {
                    ValidStrings = schema.Enum.Select(j => ((JValue)j).Value as string ?? throw new ApplicationException($"Invalid enum: {j}")).ToArray()
                },
                JSchemaType.String => new StringProperty(model, parent, key, required) {
                    MinLength = (int?)schema.MinimumLength,
                    MaxLength = (int?)schema.MaximumLength,
                    Pattern = schema.Pattern
                },
                JSchemaType.Integer => new NumberProperty(model, parent, key, required) {
                    Minimum = schema.Minimum,
                    Maximum = schema.Maximum
                },
                JSchemaType.Boolean => new BooleanProperty(model, parent, key, required),
                JSchemaType.Object => new ObjectProperty(model, parent, key, required)
                {
                    ObjectSchema = schema,
                },
                //TODO implement array as list with buttons to edit/move up/down/new/delete/duplicate (labelled as value or Object Type Name with preview)
                _ => new UnsupportedProperty(model, parent, key, required)
                //TODO implement "oneOf" types
            };
            property.Include = required || parent.ContainsKey(key);
            return property;
        }
    }
}
