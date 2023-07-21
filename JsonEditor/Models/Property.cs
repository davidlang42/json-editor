using JsonEditor.Converters;
using Microsoft.Extensions.Options;
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

        public static Property For(JObject parent, string key, JSchema schema, bool required)
        {
            Property property = schema.Type switch
            {
                JSchemaType.String when schema.Enum.Count > 0 => new EnumStringProperty(parent, key, required) {
                    ValidStrings = schema.Enum.Select(j => ((JValue)j).Value as string ?? throw new ApplicationException($"Invalid enum: {j}")).ToArray()
                },
                JSchemaType.String => new StringProperty(parent, key, required) {
                    //TODO implement min/max length
                    //TODO implement pattern validation
                },
                JSchemaType.Integer => new NumberProperty(parent, key, required) {
                    Minimum = schema.Minimum,
                    Maximum = schema.Maximum
                },
                JSchemaType.Boolean => new BooleanProperty(parent, key, required),
                //TODO implement object as button to edit (labelled as Object Type Name) with text json preview*
                //TODO implement array as list with buttons to edit/move up/down/new/delete/duplicate (labelled as value or Object Type Name with preview*)
                //*preview should be comma separated values, ignoring key names, traversing objects deeply
                _ => new UnsupportedProperty(parent, key, required)
            };
            property.Include = required || parent.ContainsKey(key);
            return property;
        }
    }
}
