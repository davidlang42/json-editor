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

        public OneOfValue(JsonModel.EditAction edit_object_action, JToken? value, JSchema[] possible_schemas)
        {
            if (possible_schemas.Length == 0)
                throw new ApplicationException("At least one JSchema must be provided.");
            PossibleValues = new Value[possible_schemas.Length];
            for (var i = 0; i < possible_schemas.Length; i++)
            {
                PossibleValues[i] = For(edit_object_action, value, possible_schemas[i]);
                if (selectedValue == null && value?.IsValid(possible_schemas[i]) == true)
                    selectedValue = PossibleValues[i];
            }
            selectedValue ??= PossibleValues.First();
        }

        public override JToken AsJToken() => SelectedValue.AsJToken();

        public override string ToString() => $"One of {PossibleValues.Length} types";

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
                return new VerticalStackLayout
                {
                    picker,
                    view
                };
            }
        }
    }
}
