using JsonEditor.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Extensions
{
    internal static class MauiExtensions
    {
        /// <summary>FlexLayouts within a grid crash with `LayoutCycleException` if the grid column gets resized to smaller than the width of its largest inner-child.
        /// This can be avoided by setting the FlexLayout's MinimumWidthRequest. This method binds the FlexLayout's MinimumWidthRequest to its children's widths.
        /// NOTE: All children must be added to the FlexLayout BEFORE this method is called.</summary>
        public static void UnfuckFlexLayout(this FlexLayout layout)
        {
            var multi = new MultiBinding
            {
                Converter = new LargestDouble()
            };
            foreach (var child in InnerChildren(layout))
                multi.Bindings.Add(new Binding
                {
                    Source = child,
                    Path = nameof(IView.Width)
                });
            layout.SetBinding(FlexLayout.MinimumWidthRequestProperty, multi);
        }

        private static IEnumerable<IView> InnerChildren(this FlexLayout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is FlexLayout nested)
                    foreach (var result in InnerChildren(nested))
                        yield return result;
                else
                    yield return child;
            }
        }
    }
}
