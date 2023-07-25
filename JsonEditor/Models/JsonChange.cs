using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class JsonChange
    {
        public string OldJson { get; }
        public string NewJson { get; }

        public bool Changed => OldJson != NewJson;

        public JsonChange(string oldJson, string newJson)
        {
            OldJson = oldJson;
            NewJson = newJson;
        }
    }
}
