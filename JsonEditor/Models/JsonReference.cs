using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public abstract class JsonReference
    {
        public JsonPath Path { get; }

        public JsonReference(JsonPath path)
        {
            Path = path;
        }

        public abstract JToken Get();
        public abstract void Set(JToken value);
    }

    public class JsonPropertyReference : JsonReference
    {
        readonly JObject parent;
        readonly string key;

        public JsonPropertyReference(JsonPath path, JObject parent, string key) : base(path)
        {
            if (!parent.ContainsKey(key))
                throw new ArgumentException("Object does not contain key", nameof(key));
            this.parent = parent;
            this.key = key;
        }

        public override JToken Get() => parent[key]!;

        public override void Set(JToken value) => parent[key] = value;
    }

    public class JsonArrayReference : JsonReference
    {
        readonly JArray parent;
        readonly int index;

        public JsonArrayReference(JsonPath path, JArray parent, int index) : base(path)
        {
            if (index < 0 || index >= parent.Count)
                throw new ArgumentException("Index out of bounds", nameof(index));
            this.parent = parent;
            this.index = index;
        }

        public override JToken Get() => parent[index];

        public override void Set(JToken value) => parent[index] = value;
    }
}
