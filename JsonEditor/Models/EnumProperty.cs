﻿using JsonEditor.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class EnumStringProperty : Property
    {
        private string? _value;
        public string? Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string[] ValidStrings { get; init; } = Array.Empty<string>();

        public EnumStringProperty(JObject parent, string key, bool required) : base(parent, key, required) { }

        public override JToken? ValueAsJToken() => Value;

        public override IView GenerateEditView()
        {
            var picker = new Picker
            {
                BindingContext = this,
                ItemsSource = ValidStrings
            };
            picker.SetBinding(Picker.SelectedItemProperty, nameof(Value));
            return picker;
        }
    }
}
