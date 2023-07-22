using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Values
{
    internal class EnumStringValue : StringValue
    {
        public string[]? ValidStrings { get; init; }

        public override View EditView
        {
            get
            {
                var picker = new Picker
                {
                    BindingContext = this,
                    ItemsSource = ValidStrings.OrThrow()
                };
                picker.SetBinding(Picker.SelectedItemProperty, nameof(Value));
                return picker;
            }
        }
    }
}
