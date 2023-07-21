using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Extensions
{
    internal static class MauiExtensions
    {
        public static void SetBindingRecursively(this BindableObject bindable, BindableProperty property, string path)
        {
            bindable.SetBinding(property, path);
            if (bindable is Layout layout)
                foreach (var child in layout.Children.OfType<BindableObject>())
                    child.SetBindingRecursively(property, path);
        }
    }
}
