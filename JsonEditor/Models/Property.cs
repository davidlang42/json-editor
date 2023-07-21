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
        public string Key { get; }
        public bool Required { get; }

        readonly JObject parent;

        public Property(JObject parent, string key, bool required)
        {
            this.parent = parent;
            Key = key;
            Required = required;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract JToken? ValueAsJToken();

        public void Commit()
        {
            var value = ValueAsJToken();
            if (value == null)
                parent.Remove(Key);
            else
                parent[Key] = value;
        }

        public abstract IView GenerateEditView();

        public IView GenerateHeaderView()
        {
            return new Label
            {
                Text = Key,
                FontAttributes = Required ? FontAttributes.Bold : FontAttributes.None
            };
        }

        public static Property For(JObject parent, string key, JSchema schema, bool required)
        {
            var token = parent[key];
            var value = (token as JValue)?.Value;
            Property property = schema.Type switch
            {
                JSchemaType.String => new StringProperty(parent, key, required) {
                    Value = value as string
                },
                JSchemaType.Integer => new NumberProperty(parent, key, required) {
                    Value = value as long?,
                    Minimum = schema.Minimum,
                    Maximum = schema.Maximum
                },
                _ => new UnsupportedProperty(parent, key, required)
                {
                    Value = token
                }
            };
            return property;
        }
    }
}
