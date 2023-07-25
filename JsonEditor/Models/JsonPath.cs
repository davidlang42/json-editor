using JsonEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Models
{
    public class JsonPath
    {
        public const string ARRAY = "[]";
        public const string ONE_OF = "|";

        public string[] Paths { get; }

        public JsonPath(string[] paths)
        {
            Paths = paths;
        }

        public override string ToString() => string.Join(".", Paths);

        public JsonPath Append(string path) => new(Paths.Concat(path.Yield()).ToArray());
    }
}
