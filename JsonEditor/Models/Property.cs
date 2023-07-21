using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public abstract class Property
    {
        public string Key { get; }

        public Property(string key)
        {
            Key = key;
        }

        public abstract string ToJson();

        public abstract IView GenerateView();

        public static Property For(string key, JSchema schema, JToken? current)
        {
            var value = (current as JValue)?.Value;
            return schema.Type switch
            {
                JSchemaType.String => new StringProperty(key) { Value = value as string ?? "" },
                JSchemaType.Integer => new NumberProperty(key) { Value = value as long? ?? 0 },
                _ => new UnsupportedProperty(key)
            };
        }
    }
}
