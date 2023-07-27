using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Converters
{
    internal class LargestDouble : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var max = values?.OfType<double>().Cast<double?>().Max();
            if (!max.HasValue || max.Value < 0)
                return 0;
            return max.Value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
