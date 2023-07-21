using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class JsonModel
    {
        public JsonFile File { get; }
        public List<Property> Properties { get; }

        public JsonModel(JsonFile file, JObject obj, JSchema schema)
        {
            File = file;
            Properties = schema.Properties.Select(i => Property.For(obj, i.Key, i.Value, schema.Required.Contains(i.Key))).ToList();
        }
    }
}
