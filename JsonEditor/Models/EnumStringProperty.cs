using JsonEditor.Converters;
using JsonEditor.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class EnumStringProperty : StringProperty
    {
        public string[]? ValidStrings { get; init; }

        public EnumStringProperty(JsonModel model, JObject parent, string key, bool required) : base(model, parent, key, required) { }

        public override VisualElement GenerateEditView()
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
