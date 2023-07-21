using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    internal class UnsupportedProperty : Property
    {
        public UnsupportedProperty(string key) : base(key) { }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }

        public override IView GenerateView()
        {
            return new Label { Text = "This type is not supported." };
        }
    }
}
