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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class OneOfValue : Value
    {
        public Value[] PossibleValues { get; }

        private Value selectedValue;
        public Value SelectedValue
        {
            get => selectedValue;
            set
            {
                if (value != selectedValue)
                {
                    selectedValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public OneOfValue(JsonModel.EditAction edit_object_action, Regex? hide_properties, Regex? name_properties, JToken? value, JSchema[] possible_schemas)
        {
            if (possible_schemas.Length == 0)
                throw new ApplicationException("At least one JSchema must be provided.");
            PossibleValues = new Value[possible_schemas.Length];
            for (var i = 0; i < possible_schemas.Length; i++)
            {
                var value_valid_for_this_option = value?.IsValid(possible_schemas[i]) == true;
                PossibleValues[i] = For(edit_object_action, hide_properties, name_properties, value_valid_for_this_option ? value : possible_schemas[i].Default?.DeepClone(), possible_schemas[i]);
                PossibleValues[i].PropertyChanged += PossibleValues_PropertyChanged;
                if (selectedValue == null && value_valid_for_this_option)
                    selectedValue = PossibleValues[i];
            }
            selectedValue ??= PossibleValues.First();
        }

        private void PossibleValues_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // assume any changes to a PossibleValue mean the SelectedValue might have changed
            NotifyPropertyChanged(nameof(SelectedValue));
        }

        public override JToken AsJToken() => SelectedValue.AsJToken();

        public override string ToString() => $"One of {PossibleValues.Length} types";

        public override IEnumerable<Value> Recurse() => this.Yield().Concat(PossibleValues.SelectMany(v => v.Recurse()));

        public override View EditView
        {
            get
            {
                var picker = new Picker
                {
                    BindingContext = this,
                    ItemsSource = PossibleValues
                };
                picker.SetBinding(Picker.SelectedItemProperty, nameof(SelectedValue));
                var view = new ContentView
                {
                    BindingContext = this
                };
                view.SetBinding(ContentView.ContentProperty, $"{nameof(SelectedValue)}.{nameof(Value.EditView)}");
                var layout = new FlexLayout
                {
                    AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Start,
                    Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
                    Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                    Children =
                    {
                        picker,
                        view
                    }
                };
                layout.UnfuckFlexLayout();
                return layout;
            }
        }
    }
}
