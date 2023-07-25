using JsonEditor.Values;
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
    public class JsonModel
    {
        public delegate void EditAction(JsonPath path, JObject obj, JSchema schema, Action refresh);

        public JsonFile File { get; }
        public JsonPath Path { get; }
        public List<Property> Properties { get; }
        public Action<JsonModel>? NavigateAction { get; set; }
        public string OriginalJson { get; }

        readonly JObject obj;
        readonly string? objectType;
        
        Action? nextRefresh = null;

        public JsonModel(JsonFile file, JsonPath path, JObject obj, JSchema schema)
        {
            File = file;
            Path = path;
            OriginalJson = obj.ToString();
            this.obj = obj;
            objectType = schema.Title;
            IEnumerable<KeyValuePair<string, JSchema>> properties_to_show = schema.Properties;
            if (File.HideProperties is Regex regex)
                properties_to_show = properties_to_show.Where(i => !regex.IsMatch(i.Key));
            Properties = properties_to_show.Select(i => new Property(this, obj, i.Key, i.Value, schema.Required.Contains(i.Key))).ToList();
        }

        public void EditObject(JsonPath path, JObject obj, JSchema schema, Action refresh)
        {
            if (nextRefresh != null)
                throw new ApplicationException("Pending refresh not run.");
            nextRefresh = refresh;
            var model = new JsonModel(File, Path.AppendReversed(path), obj, schema);
            NavigateAction?.Invoke(model);
        }

        /// <summary>Returns true if this commit made any changes</summary>
        public bool Commit(out string post_commit_json)
        {
            var pre_commit = obj.ToString();
            foreach (var property in Properties)
                property.Commit();
            post_commit_json = obj.ToString();
            return post_commit_json != pre_commit;
        }

        /// <summary>Returns true if this object has changed since the model was created.
        /// NOTE: This could be direct changes on this model, or changes to sub-objects.</summary>
        public bool HasChanged(out string new_json)
        {
            new_json = obj.ToString();
            return new_json != OriginalJson;
        }

        public JObject CloneObject() => (JObject)obj.DeepClone();

        public void Refresh()
        {
            if (nextRefresh != null)
            {
                nextRefresh();
                nextRefresh = null;
            }
        }

        /// <summary>Find objects of the same type as this one, which were the same at the time this model was created</summary>
        public JsonReference[] FindMatchingObjects()
        {
            if (objectType == null)
                return Array.Empty<JsonReference>();
            return File.FindObjectPaths(objectType, Path) // same object type but different path
                .Where(j => j.Get().ToString() == OriginalJson) // same json as this object was originally
                .ToArray();
        }
    }
}
