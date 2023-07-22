using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Extensions
{
    internal static class ExceptionExtensions
    {
        public static T OrThrow<T>(this T? value, [CallerArgumentExpression(nameof(value))] string expression = "")
        {
            return value ?? throw new ApplicationException($"{expression} not set");
        }
    }
}
