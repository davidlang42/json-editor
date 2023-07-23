using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class FilePaths : INotifyPropertyChanged
    {
        private string schemaFile = "";
        public string SchemaFile
        {
            get => schemaFile;
            set
            {
                if (value != schemaFile)
                {
                    schemaFile = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string jsonFile = "";
        public string JsonFile
        {
            get => jsonFile;
            set
            {
                if (value != jsonFile)
                {
                    jsonFile = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        const string SCHEMA_FILE_KEY = "LastSchemaFile";
        const string JSON_FILE_KEY = "LastJsonFile";

        public void SaveToUserPreferences()
        {
            Preferences.Default.Set(SCHEMA_FILE_KEY, SchemaFile);
            Preferences.Default.Set(JSON_FILE_KEY, JsonFile);
        }

        public static FilePaths LoadFromUserPreferences() => new FilePaths
        {
            SchemaFile = Preferences.Default.Get(SCHEMA_FILE_KEY, ""),
            JsonFile = Preferences.Default.Get(JSON_FILE_KEY, ""),
        };

        public static void ClearUserPreferences()
        {
            Preferences.Default.Remove(SCHEMA_FILE_KEY);
            Preferences.Default.Remove(JSON_FILE_KEY);
        }
    }
}
