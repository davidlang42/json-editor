using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class UnsupportedProperty : Property
    {
        public JToken Value { get; init; }

        public UnsupportedProperty(JsonModel model, JObject parent, string key, bool required) : base(model, parent, key, required)
        {
            Value = parent[key] ?? JValue.CreateNull();
        }

        public override JToken ValueAsJToken() => Value;

        public override VisualElement GenerateEditView()
        {
            return new Label {
                Text = $"Unsupported type: {PreviewJson(Value)}",
                LineBreakMode = LineBreakMode.NoWrap
            };
        }
    }
}
