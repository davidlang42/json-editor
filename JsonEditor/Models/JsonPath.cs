using JsonEditor.Extensions;
using Newtonsoft.Json.Linq;
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

        public JsonPath Prepend(string path)
        {
            CheckValid(path);
            return new(path.Yield().Concat(Paths).ToArray());
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

        public bool StartsWith(JsonPath other)
        {
            if (other.Paths.Length < Paths.Length)
                return false;
            for (var i = 0; i < Paths.Length; i++)
                if (Paths[i] != other.Paths[i])
                    return false;
            return true;
        }

        public IEnumerable<JsonReference> Follow(JObject root, JsonPath? exclude = null)
        {
            if (Paths.Length == 0 || (exclude != null && StartsWith(exclude))) // its super important that this condition is StartsWith() and not Equal() because if you updated an object of the same type inside itself, very bad things would happen
                return Enumerable.Empty<JsonReference>();
            return Follow(1, root, exclude); // start at 1 because the root name doesn't count
        }

        private IEnumerable<JsonReference> Follow(int path_index, JToken current, JsonPath? exclude)
        {
            if (path_index == Paths.Length)
            {
                throw new ApplicationException("Failed to stop recursion");
            }
            else
            {
                var path = Paths[path_index];
                var final = path_index == Paths.Length - 1;
                if (path.StartsWith(ARRAY_START) && path.EndsWith(ARRAY_END))
                {
                    if (current is not JArray array)
                        yield break; // path failed: can't index a non-array
                    if (path.Length == 2)
                    {
                        // create new paths for each element in the array and try them all
                        for (var i = 0; i < array.Count; i++)
                        {
                            var path_until_here = new JsonPath(Paths.Take(path_index).ToArray());
                            var path_here_indexed = path_until_here.Array(i);
                            var new_complete_path = new JsonPath(path_here_indexed.Paths.Concat(Paths.Skip(path_index + 1)).ToArray());
                            if (exclude == null || !new_complete_path.StartsWith(exclude))
                            {
                                if (final)
                                {
                                    yield return new JsonArrayReference(new_complete_path, array, i); // path finished
                                }
                                else
                                {
                                    foreach (var result in new_complete_path.Follow(path_index + 1, array[i], exclude))
                                        yield return result; // recurse on new JsonPaths
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        var index = int.Parse(path.Substring(1, path.Length - 2));
                        if (index < 0 || index >= array.Count)
                            yield break; // path failed: index out of bounds
                        if (final)
                        {
                            yield return new JsonArrayReference(this, array, index); // path finished
                        }
                        else
                        {
                            foreach (var result in Follow(path_index + 1, array[index], exclude))
                                yield return result; // recurse
                        }
                    }
                }
                else
                {
                    if (current is not JObject obj)
                        yield break; // path failed: can't access a property of a non-object
                    if (obj[path] is not JToken token)
                        yield break; // path failed: property didn't exist on this object
                    if (final)
                    {
                        yield return new JsonPropertyReference(this, obj, path); // path finished
                    }
                    else
                    {
                        foreach (var result in Follow(path_index + 1, token, exclude))
                            yield return result; // recurse
                    }
                }
            }
        }
    }
}
