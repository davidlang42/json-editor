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
        const string ARRAY = "[]";
        const string ONE_OF = "|";

        public string[] Paths { get; }

        public JsonPath(string first_path) : this(new[] { first_path })
        { }

        private JsonPath(string[] paths)
        {
            Paths = paths;
        }

        public override string ToString() => string.Join(".", Paths);

        public JsonPath Append(string path)
        {
            if (path == ARRAY)
                throw new ApplicationException($"{nameof(path)} cannot be the reserved array value: {ARRAY}");
            if (path == ONE_OF)
                throw new ApplicationException($"{nameof(path)} cannot be the reserved oneOf value: {ONE_OF}");
            return new(Paths.Concat(path.Yield()).ToArray());
        }

        public JsonPath Array() => new(Paths.Concat(ARRAY.Yield()).ToArray());

        public JsonPath OneOf() => new(Paths.Concat(ONE_OF.Yield()).ToArray());
    }
}
