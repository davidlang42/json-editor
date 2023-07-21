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
        public List<Property> Properties { get; }

        public JsonModel(JObject obj, JSchema schema)
        {
            Properties = schema.Properties.Select(i => Property.For(i.Key, i.Value, obj[i.Key])).ToList();
        }
    }
}
