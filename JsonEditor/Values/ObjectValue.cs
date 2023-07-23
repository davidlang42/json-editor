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

        readonly JsonModel.EditAction editAction;

        public ObjectValue(JsonModel.EditAction edit_action)
        {
            editAction = edit_action;
        }

        public override JToken AsJToken() => Value;

        public override string ToString() => ObjectType ?? "Object";

        public override View EditView
        {
            get
            {
                var copy = new Button
                {
                    Text = "🗐",
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.LightGray
                };
                copy.Clicked += Copy_Clicked;
                var paste = new Button
                {
                    Text = "📋",
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.LightGray
                };
                paste.Clicked += Paste_Clicked;
                var button = new Button
                {
                    Text = $"Edit {this}"
                };
                button.Clicked += Edit_Clicked;
                var label = new Label
                {
                    BindingContext = this,
                    LineBreakMode = LineBreakMode.NoWrap
                };
                label.SetBinding(Label.TextProperty, nameof(Value), converter: new JsonPreview());
                var grid = new Grid
                {
                    ColumnSpacing = 5,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star)
                    }
                };
                grid.Add(button);
                grid.Add(copy, 1);
                grid.Add(paste, 2);
                grid.Add(label, 3);
                return grid;
            }
        }

        private void Edit_Clicked(object? sender, EventArgs e)
        {
            editAction("", Value, ObjectSchema.OrThrow(), () => NotifyPropertyChanged(nameof(Value)));
        }

        private async void Copy_Clicked(object? sender, EventArgs e)
        {
            await Clipboard.Default.SetTextAsync(Value.ToString(Formatting.Indented));
        }

        private async void Paste_Clicked(object? sender, EventArgs e)
        {
            if (await Clipboard.Default.GetTextAsync() is string json && JObject.Parse(json) is JObject parsed)
                Value = parsed;
        }
    }
}
