using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Converters
{
    internal class BoolToColor : IValueConverter
    {
        Color trueColor, falseColor;

        public BoolToColor(Color trueColor, Color falseColor)
        {
            this.trueColor = trueColor;
            this.falseColor = falseColor;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? trueColor : falseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
