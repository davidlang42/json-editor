using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class ObjectValue : Value
    {
        private JObject _value = new JObject();
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

        public string? ObjectType { get; init; }
        public JSchema? ObjectSchema { get; init; }

        readonly Action<JObject, JSchema> editAction;

        public ObjectValue(Action<JObject, JSchema> edit_action)
        {
            editAction = edit_action;
        }

        public override JToken AsJToken() => Value;

        public override View EditView
        {
            get
            {
                var button = new Button
                {
                    BindingContext = this,
                    Text = ObjectType ?? "Object"
                };
                button.Clicked += Button_Clicked;
                var label = new Label
                {
                    Text = Value.ToString(Formatting.None).Truncate(400),
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
        }

        private void Button_Clicked(object? sender, EventArgs e)
        {
            editAction(Value, ObjectSchema.OrThrow());
        }
    }
}
