using CommunityToolkit.Maui.Core.Extensions;
using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class ArrayValue : Value
    {
        public ObservableCollection<Value> Items { get; }

        public long? MinItems { get; }
        public long? MaxItems { get; }

        public bool IsFixedSize() => MinItems.HasValue && MaxItems.HasValue && MinItems.Value == MaxItems.Value;

        private JsonModel.EditAction editObjectAction;
        private JSchema itemSchema;

        public ArrayValue(JsonModel.EditAction edit_object_action, JArray array, JSchema item_schema, long? min_items, long? max_items)
        {
            editObjectAction = edit_object_action;
            itemSchema = item_schema;
            MinItems = min_items;
            MaxItems = max_items;
            Items = new ObservableCollection<Value>();
            for (var i = 0; i < array.Count; i++)
            {
                if (MaxItems.HasValue && i == MaxItems.Value)
                    break;
                Items.Add(MakeNewItem(array[i]));
            }
            if (MinItems.HasValue)
            {
                var template = array.Last ?? throw new ApplicationException("Array must contain at least one element when MinItems > 0");
                while (Items.Count < MinItems.Value)
                    Items.Add(MakeNewItem(template.DeepClone()));
            }
        }

        private Value MakeNewItem(JToken token)
        {
            Value? v = null;
            v = For((p, o, s, r) => editObjectAction($"[{Items.IndexOf(v!)}]{p}", o, s, r), token, itemSchema);
            return v;
        }

        public override JToken AsJToken()
        {
            var array = new JArray();
            foreach (var value in Items)
                array.Add(value.AsJToken());
            return array;
        }

        public override string ToString()
        {
            var item_name = Items.FirstOrDefault()?.ToString() ?? "items";
            var range_name = (MinItems, MaxItems) switch
            {
                (long min, long max) when min == max => $"{min} ",
                (long min, long max) => $"{min}-{max} ",
                (null, long max) => $"up to {max} ",
                (long min, null) => $"at least {min} ",
                (null, null) => ""
            };
            return $"Array of {range_name}{item_name}";
        }

        public override View EditView
        {
            get
            {
                return new Frame
                {
                    BorderColor = Colors.Black,
                    Content = new CollectionView
                    {
                        ItemsSource = Items,
                        ItemTemplate = new DataTemplate(GenerateDataTemplate),
                    }
                };
            }
        }
        
        private object GenerateDataTemplate()
        {
            // BindingContext will be set to a Value
            var layout = new HorizontalStackLayout
            {
                ArrayButton("↑", MoveUp_Clicked),//TODO disable at top
                ArrayButton("↓", MoveDown_Clicked)//TODO disable at bottom
            };
            if (!IsFixedSize())
            {
                layout.Add(ArrayButton("✗", Remove_Clicked)); //TODO disable if going to break min/max bounds
                layout.Add(ArrayButton("+", Duplicate_Clicked)); //TODO disable if going to break min/max bounds
            };
            var label = new Label { Text = "[?]" }; //TODO make label show array index
            layout.Add(label);
            var view = new ContentView();
            view.SetBinding(ContentView.ContentProperty, nameof(Value.EditView));
            layout.Add(view);
            return layout;
        }

        static Button ArrayButton(string text, EventHandler handler)
        {
            var button = new Button
            {
                Text = text,
                BackgroundColor = Colors.Orange
            };
            button.Clicked += handler;
            return button;
        }

        private void MoveUp_Clicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Value value)
            {
                var index = Items.IndexOf(value);
                if (index > 0)
                    Items.Move(index, index - 1);
            }
        }

        private void MoveDown_Clicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Value value)
            {
                var index = Items.IndexOf(value);
                if (index < Items.Count - 1)
                    Items.Move(index, index + 1);
            }
        }

        private void Remove_Clicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Value value && (!MinItems.HasValue || Items.Count > MinItems.Value))
                Items.Remove(value);
        }

        private void Duplicate_Clicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Value value && (!MaxItems.HasValue || Items.Count < MaxItems.Value))
            {
                var index = Items.IndexOf(value);
                var new_item = MakeNewItem(value.AsJToken().DeepClone());
                Items.Insert(index, new_item);
            }
        }
    }
}
