﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class UnsupportedProperty : Property
    {
        public JToken? Value { get; init; }

        public UnsupportedProperty(JObject parent, string key, bool required) : base(parent, key, required) { }

        public override JToken? ValueAsJToken() => Value;

        public override IView GenerateEditView()
        {
            return new Label { Text = Value?.ToString() ?? "This type is not supported." };
        }
    }
}
