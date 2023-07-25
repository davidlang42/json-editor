using Newtonsoft.Json;
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
            File.WriteAllText(Filename, Root.ToString(Formatting.Indented));
        }

        public static JsonFile Load(string schemaFile, string jsonFile)
        {
            var schema = JSchema.Parse(AddDefinitionTitlesToSchema(File.ReadAllText(schemaFile)));
            var root = JObject.Parse(File.ReadAllText(jsonFile));
            return new(jsonFile, root, schema);
        }

        static string AddDefinitionTitlesToSchema(string schema_json)
        {
            var obj = JObject.Parse(schema_json);
            if (obj["definitions"] is JObject definitons)
            {
                foreach (var (key, value) in definitons)
                    if (value is JObject sub_schema && !sub_schema.ContainsKey("title"))
                        sub_schema["title"] = key; // if no title is set, use the definitions key as the title
                return obj.ToString(Formatting.None);
            }
            else
            {
                // No titles to add from definitions
                return schema_json;
            }
        }
    }
}
