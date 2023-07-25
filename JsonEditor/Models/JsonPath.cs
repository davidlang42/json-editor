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
        const char SEPARATOR = '.';
        const char ARRAY_START = '[';
        const char ARRAY_END = ']';

        public string[] Paths { get; }

        public JsonPath() : this(new string[0])
        { }

        public JsonPath(string first_path) : this(new[] { first_path })
        {
            CheckValid(first_path);
        }

        private JsonPath(string[] paths)
        {
            Paths = paths;
        }

        public override string ToString() => string.Join(SEPARATOR, Paths);

        public JsonPath AppendReversed(JsonPath reverse) => new(Paths.Concat(reverse.Paths.Reverse()).ToArray());

        public JsonPath Append(string path)
        {
            CheckValid(path);
            return new(Paths.Concat(path.Yield()).ToArray());
        }

        private static void CheckValid(string path)
        {
            if (path.Contains(SEPARATOR))
                throw new ApplicationException($"New paths cannot contain the reserved separator character: {SEPARATOR}");
            if (path.Contains(ARRAY_START))
                throw new ApplicationException($"New paths cannot contain the reserved array character: {ARRAY_START}");
            if (path.Contains(ARRAY_END))
                throw new ApplicationException($"New paths cannot contain the reserved array character: {ARRAY_END}");
        }

        public JsonPath Array(int? index = null) => new(Paths.Concat($"{ARRAY_START}{index}{ARRAY_END}".Yield()).ToArray());
    }
}
