using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Extensions
{
    internal static class StringExtensions
    {
        public static string Truncate(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || str.Length < length)
                return str;
            return str.Substring(0, length);
        }
    }
}
