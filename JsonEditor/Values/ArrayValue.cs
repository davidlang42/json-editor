using CommunityToolkit.Maui.Core.Extensions;
using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Microsoft.Maui.Controls.Shapes;
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

        public bool CanAdd => !MaxItems.HasValue || Items.Count < MaxItems.Value;
        public bool CanRemove => Items.Count > (MinItems ?? 0);

        private JsonModel.EditAction editObjectAction;
        private JSchema itemSchema;

        public ArrayValue(JsonModel.EditAction edit_object_action, JArray array, JSchema item_schema, long? min_items, long? max_items)
        {
            editObjectAction = edit_object_action;
            itemSchema = item_schema;
            MinItems = min_items;
            MaxItems = max_items;
            Items = new ObservableCollection<Value>();
            Items.CollectionChanged += Items_CollectionChanged;
            for (var i = 0; i < array.Count; i++)
            {
                if (MaxItems.HasValue && i == MaxItems.Value)
                    break;
                Items.Add(MakeNewItem(array[i]));
            }
            if (MinItems.HasValue)
            {
                var template = item_schema.Default ?? array.Last ?? new JArray();
                while (Items.Count < MinItems.Value)
                    Items.Add(MakeNewItem(template.DeepClone()));
            }
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(CanAdd));
            NotifyPropertyChanged(nameof(CanRemove));
        }

        private Value MakeNewItem(JToken token)
        {
            Value? v = null;
            v = For((p, o, s) => editObjectAction(p.Array(Items.IndexOf(v!)), o, s), token, itemSchema);
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

        public override IEnumerable<Value> Recurse() => this.Yield().Concat(Items.SelectMany(v => v.Recurse()));

        public override View EditView
        {
            get
            {
                var layout = new VerticalStackLayout
                {
                    new CollectionView
                    {
                        ItemsSource = Items,
                        ItemTemplate = new DataTemplate(GenerateDataTemplate),
                    }
                };
                if (!IsFixedSize())
                {
                    var add_button = ArrayButton("Add blank", Add_Clicked, nameof(CanAdd));
                    var binding = new Binding
                    {
                        Source = Items,
                        Path = nameof(ObservableCollection<Value>.Count),
                        Converter = new IntIsZero()
                    };
                    add_button.SetBinding(Button.IsVisibleProperty, binding);
                    layout.Add(new HorizontalStackLayout { add_button });
                }
                return new Border
                {
                    Stroke = Brush.Black,
                    Padding = 1,
                    Content = layout
                };
            }
        }
        
        private object GenerateDataTemplate()
        {
            // BindingContext will be set to a Value
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                }
            };
            grid.Add(ArrayButton("↑", MoveUp_Clicked));
            grid.Add(ArrayButton("↓", MoveDown_Clicked), 1);
            if (!IsFixedSize())
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.Add(ArrayButton("✗", Remove_Clicked, nameof(CanRemove)), 2);
                grid.Add(ArrayButton("+", Duplicate_Clicked, nameof(CanAdd)), 3);
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            var view = new ContentView();
            view.SetBinding(ContentView.ContentProperty, nameof(Value.EditView));
            grid.Add(view, grid.ColumnDefinitions.Count - 1);
            return grid;
        }

        Button ArrayButton(string text, EventHandler handler, string? enabled_binding_path = null)
        {
            var button = new Button
            {
                Text = text,
                BackgroundColor = Colors.Orange
            };
            button.Clicked += handler;
            if (enabled_binding_path != null)
            {
                var binding = new Binding
                {
                    Source = this,
                    Path = enabled_binding_path
                };
                button.SetBinding(Button.IsEnabledProperty, binding);
            }
            return button;
        }

        private void MoveUp_Clicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Value value)
            {
                var index = Items.IndexOf(value);
                if (index == -1)
                    return;
                if (index > 0)
                    Items.Move(index, index - 1);
            }
        }

        private void MoveDown_Clicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Value value)
            {
                var index = Items.IndexOf(value);
                if (index == -1)
                    return;
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
                if (index == -1)
                    return;
                var new_item = MakeNewItem(value.AsJToken().DeepClone());
                Items.Insert(index, new_item);
            }
        }

        private void Add_Clicked(object? sender, EventArgs e)
        {
            if (!MaxItems.HasValue || Items.Count < MaxItems.Value)
                Items.Add(MakeNewItem(itemSchema.Default?.DeepClone() ?? new JArray()));
        }
    }
}
