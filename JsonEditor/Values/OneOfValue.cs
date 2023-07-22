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

        public OneOfValue(Action<JObject, JSchema> edit_object_action, JToken? value, JSchema[] possible_schemas)
        {
            if (possible_schemas.Length == 0)
                throw new ApplicationException("At least one JSchema must be provided.");
            PossibleValues = possible_schemas.Select(s => For(edit_object_action, value, s)).ToArray();
            selectedValue = PossibleValues.FirstOrDefault(v => v.AsJToken().Type != JTokenType.Null) ?? PossibleValues.First(); //TODO this isn't working to select the current value
        }

        public override JToken AsJToken() => SelectedValue.AsJToken(); //TODO clicking save on one option of a oneOf type doesn't save it or update it back on the previous page

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
