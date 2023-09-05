using JsonEditor.Extensions;
using JsonEditor.Values;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class JsonModel : INotifyPropertyChanged
    {
        public delegate void EditAction(JsonPath path, JObject obj, JSchema schema);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public JsonFile File { get; }
        public JsonPath Path { get; }
        public List<Property> Properties { get; }
        public Action<JsonModel>? NavigateAction { get; set; }
        public string OriginalJson { get; }

        public bool AnyChanges => Properties.Any(p => p.ChangesToCommit);

        readonly JObject obj;
        readonly string? objectType;
        
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
            Properties = new List<Property>();
            foreach (var i in properties_to_show)
            {
                var p = new Property(this, obj, i.Key, i.Value, schema.Required.Contains(i.Key));
                p.PropertyChanged += Properties_PropertyChanged;
                Properties.Add(p);
            }
        }

        private void Properties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Property.ChangesToCommit))
            {
                NotifyPropertyChanged(nameof(AnyChanges));
            }
        }

        public void EditObject(JsonPath path, JObject obj, JSchema schema)
        {
            if (File.ShortcutSingleObjectProperties
                && schema.Properties.SingleOrDefaultSafe() is (string single_name, JSchema single_schema)
                && single_schema.Type == JSchemaType.Object
                && obj.TryGetValue(single_name, out var existing_value)
                && existing_value is JObject existing_object)
            {
                // special case: automatically navigate into single object properties as long as they have a value
                EditObject(path.Prepend(single_name), existing_object, single_schema);
            }
            else
            {
                // normal case: edit the object we were given
                var model = new JsonModel(File, Path.AppendReversed(path), obj, schema);
                NavigateAction?.Invoke(model);
            }
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

        public JObject CloneObject() => (JObject)obj.DeepClone();

        public void Refresh()
        {
            foreach (var property in Properties)
                foreach (var value in property.Value.Recurse().OfType<ObjectValue>())
                    value.Refresh(); // all objects could have changed due to "update matching objects"
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
