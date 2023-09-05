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
        const string DEFAULT_SCHEMA_FILE = "";
        private string schemaFile = DEFAULT_SCHEMA_FILE;
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

        const string DEFAULT_JSON_FILE = "";
        private string jsonFile = DEFAULT_JSON_FILE;
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

        const string DEFAULT_HIDE_PROPERTIES_REGEX = "";
        private string hidePropertiesRegex = DEFAULT_HIDE_PROPERTIES_REGEX;
        public string HidePropertiesRegex
        {
            get => hidePropertiesRegex;
            set
            {
                if (value != hidePropertiesRegex)
                {
                    hidePropertiesRegex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        const string DEFAULT_NAME_PROPERTIES_REGEX = "^name";
        private string namePropertiesRegex = DEFAULT_NAME_PROPERTIES_REGEX;
        public string NamePropertiesRegex
        {
            get => namePropertiesRegex;
            set
            {
                if (value != namePropertiesRegex)
                {
                    namePropertiesRegex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        const bool DEFAULT_OFFER_COMMON_OBJECT_UPDATES = false;
        private bool offerCommonObjectUpdates = DEFAULT_OFFER_COMMON_OBJECT_UPDATES;
        public bool OfferCommonObjectUpdates
        {
            get => offerCommonObjectUpdates;
            set
            {
                if (value != offerCommonObjectUpdates)
                {
                    offerCommonObjectUpdates = value;
                    NotifyPropertyChanged();
                }
            }
        }

        const bool DEFAULT_SHORTCUT_SINGLE_OBJECT_PROPERTIES = true;
        private bool shortcutSingleObjectProperties = DEFAULT_SHORTCUT_SINGLE_OBJECT_PROPERTIES;
        public bool ShortcutSingleObjectProperties
        {
            get => shortcutSingleObjectProperties;
            set
            {
                if (value != shortcutSingleObjectProperties)
                {
                    shortcutSingleObjectProperties = value;
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
        const string HIDE_PROPERTIES_KEY = "LastHidePropertiesRegex";
        const string NAME_PROPERTIES_KEY = "LastNamePropertiesRegex";
        const string OFFER_COMMON_OBJECT_UPDATES_KEY = "LastOfferCommonObjectUpdates";
        const string SHORTCUT_SINGLE_OBJECT_PROPERTIES_KEY = "LastShortcutSingleObjectProperties";

        public void SaveToUserPreferences()
        {
            Preferences.Default.Set(SCHEMA_FILE_KEY, SchemaFile);
            Preferences.Default.Set(JSON_FILE_KEY, JsonFile);
            Preferences.Default.Set(HIDE_PROPERTIES_KEY, HidePropertiesRegex);
            Preferences.Default.Set(NAME_PROPERTIES_KEY, NamePropertiesRegex);
            Preferences.Default.Set(OFFER_COMMON_OBJECT_UPDATES_KEY, OfferCommonObjectUpdates);
            Preferences.Default.Set(SHORTCUT_SINGLE_OBJECT_PROPERTIES_KEY, ShortcutSingleObjectProperties);
        }

        public static FilePaths LoadFromUserPreferences() => new FilePaths
        {
            SchemaFile = Preferences.Default.Get(SCHEMA_FILE_KEY, DEFAULT_SCHEMA_FILE),
            JsonFile = Preferences.Default.Get(JSON_FILE_KEY, DEFAULT_JSON_FILE),
            HidePropertiesRegex = Preferences.Default.Get(HIDE_PROPERTIES_KEY, DEFAULT_HIDE_PROPERTIES_REGEX),
            NamePropertiesRegex = Preferences.Default.Get(NAME_PROPERTIES_KEY, DEFAULT_NAME_PROPERTIES_REGEX),
            OfferCommonObjectUpdates = Preferences.Default.Get(OFFER_COMMON_OBJECT_UPDATES_KEY, DEFAULT_OFFER_COMMON_OBJECT_UPDATES),
            ShortcutSingleObjectProperties = Preferences.Default.Get(SHORTCUT_SINGLE_OBJECT_PROPERTIES_KEY, DEFAULT_SHORTCUT_SINGLE_OBJECT_PROPERTIES),
        };

        public static void ClearUserPreferences()
        {
            Preferences.Default.Remove(SCHEMA_FILE_KEY);
            Preferences.Default.Remove(JSON_FILE_KEY);
            Preferences.Default.Remove(HIDE_PROPERTIES_KEY);
            Preferences.Default.Remove(NAME_PROPERTIES_KEY);
            Preferences.Default.Remove(OFFER_COMMON_OBJECT_UPDATES_KEY);
            Preferences.Default.Remove(SHORTCUT_SINGLE_OBJECT_PROPERTIES_KEY);
        }
    }
}
