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
using System.Text.RegularExpressions;
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
        readonly Regex? hideProperties;
        readonly Regex? nameProperties;

        public ObjectValue(JsonModel.EditAction edit_action, Regex? hide_properties, Regex? name_properties)
        {
            editAction = edit_action;
            hideProperties = hide_properties;
            nameProperties = name_properties;
        }

        public override JToken AsJToken() => Value;

        public override string ToString()
        {
            if (ObjectType is string title)
                return title;
            if (ObjectSchema?.Required.SingleOrDefaultSafe() is string one_required_property)
                return one_required_property;
            if (ObjectSchema?.Properties.Keys.SingleOrDefaultSafe() is string one_defined_property)
                return one_defined_property;
            return "Object";
        }

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
                    BindingContext = this
                };
                button.SetBinding(Button.TextProperty, new Binding
                {
                    Path = nameof(Value),
                    Converter = new JsonPreview
                    {
                        IncludeOnlyProperties = nameProperties,
                        MaximumSegements = 1,
                        FallbackValue = ToString()
                    },
                    StringFormat = "Edit {0}"
                });
                button.Clicked += Edit_Clicked;
                button.SetBinding(ToolTipProperties.TextProperty, nameof(Value), converter: new JsonPreview
                {
                    DontTraverseProperties = hideProperties,
                    ExcludeProperties = hideProperties,
                    Schema = ObjectSchema
                });
                var layout = new FlexLayout
                {
                    AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Start,
                    Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
                    Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                    Children =
                    {
                        button,
                        copy,
                        paste
                    }
                };
                layout.UnfuckFlexLayout();
                return layout;
            }
        }

        private void Edit_Clicked(object? sender, EventArgs e)
        {
            editAction(new(), Value, ObjectSchema.OrThrow());
        }

        public void Refresh()
        {
            NotifyPropertyChanged(nameof(Value));
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
