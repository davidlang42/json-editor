﻿using JsonEditor.Converters;
using JsonEditor.Extensions;
using JsonEditor.Values;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class Property : INotifyPropertyChanged
    {
        public JsonModel Model { get; }
        public string Label { get; }
        public bool Required { get; }
        public Value Value { get; }

        private bool _include = true;
        public bool Include
        {
            get => _include;
            set
            {
                if (value != _include)
                {
                    _include = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        readonly JObject parent;

        public Property(JsonModel model, JObject parent, string key, JSchema schema, bool required)
        {
            Model = model;
            this.parent = parent;
            Label = key;
            Required = required;
            Value = Value.For(Model.EditObject, parent[key], schema);
            Include = required || parent.ContainsKey(key);
        }

        public void Commit()
        {
            if (Include)
                parent[Label] = Value.AsJToken();
            else
                parent.Remove(Label);
        }

        public IView GenerateHeaderView() //TODO this doesn't really belong here
        {
            //TODO right click property header > copy, paste (json as text)
            return new Label
            {
                Text = Label,
                FontAttributes = Required ? FontAttributes.Bold : FontAttributes.None
            };
        }
    }
}
