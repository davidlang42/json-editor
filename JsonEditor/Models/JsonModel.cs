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
        public delegate void EditAction(string path, JObject obj, JSchema schema, Action refresh);

        public string Title { get; }
        public JsonFile File { get; }
        public List<Property> Properties { get; }
        public Action<JsonModel>? NavigateAction { get; set; }

        Action? nextRefresh = null;

        public JsonModel(JsonFile file, string title, JObject obj, JSchema schema)
        {
            Title = title;
            File = file;
            IEnumerable<KeyValuePair<string, JSchema>> properties_to_show = schema.Properties;
            if (File.HideProperties is Regex regex)
                properties_to_show = properties_to_show.Where(i => !regex.IsMatch(i.Key));
            Properties = properties_to_show.Select(i => new Property(this, obj, i.Key, i.Value, schema.Required.Contains(i.Key))).ToList();
        }

        public void EditObject(string path, JObject obj, JSchema schema, Action refresh)
        {
            if (nextRefresh != null)
                throw new ApplicationException("Pending refresh not run.");
            nextRefresh = refresh;
            var model = new JsonModel(File, $"{Title}.{path}", obj, schema);
            NavigateAction?.Invoke(model);
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
