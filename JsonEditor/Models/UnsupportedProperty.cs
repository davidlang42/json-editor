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
        readonly string? rawJson;

        public UnsupportedProperty(string key, string? raw_json) : base(key)
        {
            rawJson = raw_json;
        }

        public override string? ToJsonAssignment() => rawJson == null ? null : $"\"{Key}\": {rawJson}";

        public override IView GenerateView()
        {
            return new Label { Text = rawJson ?? "This type is not supported." };
        }
    }
}
