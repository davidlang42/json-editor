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
    }
}
