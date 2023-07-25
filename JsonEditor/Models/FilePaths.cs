﻿using System;
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

        private string hidePropertiesRegex = "";
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        const string SCHEMA_FILE_KEY = "LastSchemaFile";
        const string JSON_FILE_KEY = "LastJsonFile";
        const string HIDE_PROPERTIES_KEY = "LastHidePropertiesRegex";

        public void SaveToUserPreferences()
        {
            Preferences.Default.Set(SCHEMA_FILE_KEY, SchemaFile);
            Preferences.Default.Set(JSON_FILE_KEY, JsonFile);
            Preferences.Default.Set(HIDE_PROPERTIES_KEY, HidePropertiesRegex);
        }

        public static FilePaths LoadFromUserPreferences() => new FilePaths
        {
            SchemaFile = Preferences.Default.Get(SCHEMA_FILE_KEY, ""),
            JsonFile = Preferences.Default.Get(JSON_FILE_KEY, ""),
            HidePropertiesRegex = Preferences.Default.Get(HIDE_PROPERTIES_KEY, "")
        };

        public static void ClearUserPreferences()
        {
            Preferences.Default.Remove(SCHEMA_FILE_KEY);
            Preferences.Default.Remove(JSON_FILE_KEY);
            Preferences.Default.Remove(HIDE_PROPERTIES_KEY);
        }
    }
}
