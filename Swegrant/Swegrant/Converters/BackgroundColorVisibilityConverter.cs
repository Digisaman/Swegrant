using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Swegrant.Converters
{
    internal class BackgroundColorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is bool)
            {
                bool isVisible = (bool)value;
                if (isVisible)
                    return new Color(0, 27, 116);
                else
                    return new Color(0, 0, 0);

            }
            return new Color(0, 0, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("ObjectBorderVisibilityConvertercan only be used OneWay.");
        }
    }
}
