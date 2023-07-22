using JsonEditor.Converters;
using JsonEditor.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class ObjectProperty : Property
    {
        private JObject _value;
        public JObject Value
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

        public string ObjectType { get; init; } = "(object)"; //TODO set object type name
        public JSchema? ObjectSchema { get; init; }

        public ObjectProperty(JsonModel model, JObject parent, string key, bool required) : base(model, parent, key, required)
        {
            _value = parent[key] as JObject ?? new JObject();
        }

        public override JToken ValueAsJToken() => Value;

        public override VisualElement GenerateEditView()
        {
            var button = new Button
            {
                BindingContext = this,
                Text = ObjectType
            };
            button.Clicked += Button_Clicked;
            var label = new Label
            {
                Text = PreviewJson(Value),
                LineBreakMode = LineBreakMode.NoWrap
            };
            var grid = new Grid
            {
                ColumnSpacing = 5,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star)
                }
            };
            grid.Add(button);
            grid.Add(label);
            grid.SetColumn(label, 1);
            return grid;
        }

        private void Button_Clicked(object? sender, EventArgs e)
        {
            Model.EditObject(Value, ObjectSchema.OrThrow());
        }
    }
}
