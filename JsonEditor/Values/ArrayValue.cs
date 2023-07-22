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

        //TODO implement min/max array items
        public long? MinItems { get; init; }
        public long? MaxItems { get; init; }

        public ArrayValue(Action<JObject, JSchema> edit_object_action, JArray array, JSchema item_schema)
        {
            Items = array.Select(t => For(edit_object_action, t, item_schema)).ToObservableCollection();
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
                //TODO adds buttons to move array items up/down, add/delete/duplicate
                var collection = new CollectionView
                {
                    ItemsSource = Items,//TODO does this need binding to get change events?
                    ItemTemplate = new DataTemplate(GenerateDataTemplate),
                };
                return collection;
            }
        }
        
        private object GenerateDataTemplate()
        {
            var view = new ContentView(); // BindingContext will be set to a Value
            view.SetBinding(ContentView.ContentProperty, nameof(Value.EditView));
            return view; //TODO add label to show array index
        }
    }
}
