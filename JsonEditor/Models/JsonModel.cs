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
        public Action<JsonModel>? NavigateAction { get; set; }

        public JsonModel(JsonFile file, JObject obj, JSchema schema)
        {
            File = file;
            Properties = schema.Properties.Select(i => new Property(this, obj, i.Key, i.Value, schema.Required.Contains(i.Key))).ToList();
        }

        public void EditObject(JObject obj, JSchema schema)
        {
            var model = new JsonModel(File, obj, schema);
            NavigateAction?.Invoke(model);
        }
    }
}
