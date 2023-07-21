using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class JsonFile
    {
        public string Filename { get; set; }
        public JObject Root { get; }
        public JSchema Schema { get; }

        public JsonFile(string filename, JObject root, JSchema schema)
        {
            Filename = filename;
            Root = root;
            Schema = schema;
        }

        public void Save()
        {
            File.WriteAllText(Filename, Root.ToString());
        }

        public static JsonFile Load(string schemaFile, string jsonFile)
        {
            var schema = JSchema.Parse(File.ReadAllText(schemaFile));
            var root = JObject.Parse(File.ReadAllText(jsonFile));
            return new(jsonFile, root, schema);
        }
    }
}
