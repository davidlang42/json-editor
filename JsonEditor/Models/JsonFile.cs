using JsonEditor.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class JsonFile
    {
        public string Filename { get; set; }
        public JObject Root { get; }
        public JSchema Schema { get; }
        public Regex? HideProperties { get; }
        public Regex? NameProperties { get; }
        public bool ShortcutSingleObjectProperties { get; }

        Dictionary<string, JsonPath[]>? objectsByType = null;

        public JsonFile(string filename, JObject root, JSchema schema, Regex? hide_properties, Regex? name_properties, bool ignore_matching_objects, bool shortcut_single_object_properties)
        {
            Filename = filename;
            Root = root;
            Schema = schema;
            HideProperties = hide_properties;
            NameProperties = name_properties;
            if (!ignore_matching_objects)
            {
                objectsByType = EnumerateObjectsByType(schema, new(schema.Title ?? "root"))
                    .GroupBy(p => p.ObjectType).ToDictionary(g => g.Key, g => g.Select(p => p.Path).ToArray());
            }
            ShortcutSingleObjectProperties = shortcut_single_object_properties;
        }

        public void Save()
        {
            File.WriteAllText(Filename, Root.ToString(Formatting.Indented));
        }

        public IEnumerable<JsonReference> FindObjectPaths(string object_type, JsonPath excluding)
        {
            // objectsByType will be null if the constructor was told to ignore matching objects
            if (objectsByType == null || !objectsByType.TryGetValue(object_type, out var schema_paths))
                yield break;
            foreach (var schema_path in schema_paths)
                foreach (var result in schema_path.Follow(Root, excluding))
                    yield return result;
        }

        public static JsonFile Load(string schemaFile, string jsonFile, Regex? hide_properties, Regex? name_regex, bool ignore_matching_objects, bool shortcut_single_object_properties)
        {
            var schema = JSchema.Parse(AddDefinitionTitlesToSchema(File.ReadAllText(schemaFile)));
            var root = JObject.Parse(File.ReadAllText(jsonFile));
            return new(jsonFile, root, schema, hide_properties, name_regex, ignore_matching_objects, shortcut_single_object_properties);
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

        static IEnumerable<(string ObjectType, JsonPath Path)> EnumerateObjectsByType(JSchema schema, JsonPath path)
        {
            switch (schema.Type)
            {
                case JSchemaType.Object:
                    if (schema.Title is string title)
                        yield return (title, path);
                    foreach (var (key, value) in schema.Properties)
                        foreach (var result in EnumerateObjectsByType(value, path.Append(key)))
                            yield return result;
                    break;

                case JSchemaType.Array when schema.Items.SingleOrDefaultSafe() is JSchema one_type_of_item:
                    foreach (var result in EnumerateObjectsByType(one_type_of_item, path.Array()))
                        yield return result;
                    break;

                case null when schema.OneOf.Count > 0:
                    foreach (var one_of in schema.OneOf)
                        foreach (var result in EnumerateObjectsByType(one_of, path))
                            yield return result; // this case has not been tested
                    break;
            }
        }
    }
}
