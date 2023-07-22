using CommunityToolkit.Maui.Behaviors;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Behaviors
{
    public class JsonValidationBehavior : ValidationBehavior
    {
        public JSchema? Schema { get; set; }

        protected override ValueTask<bool> ValidateAsync(object? value, CancellationToken token)
        {
            if (value is not string str)
                return ValueTask.FromResult(false);
            try
            {
                if (JToken.Parse(str) is not JToken jtoken)
                    return ValueTask.FromResult(false);
                if (Schema is JSchema schema && !jtoken.IsValid(schema))
                    return ValueTask.FromResult(false);
            }
            catch
            {
                return ValueTask.FromResult(false);
            }
            return ValueTask.FromResult(true);
        }
    }
}
