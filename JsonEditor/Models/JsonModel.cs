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

        readonly JObject obj;
        readonly string? objectType;
        Action? nextRefresh = null;

        public JsonModel(JsonFile file, JsonPath path, JObject obj, JSchema schema)
        {
            File = file;
            Path = path;
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

        public JsonChange Commit()
        {
            var old_json = obj.ToString();
            foreach (var property in Properties)
                property.Commit();
            var new_json = obj.ToString();
            return new(old_json, new_json);
        }

        public void Refresh()
        {
            if (nextRefresh != null)
            {
                nextRefresh();
                nextRefresh = null;
            }
        }
    }
}
