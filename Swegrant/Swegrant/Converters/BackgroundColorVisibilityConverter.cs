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
                {
                    Color BackColor = Color.FromHex("001b74");
                    return BackColor;
                }
                else
                    return Color.Black;

            }
            return Color.Black;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("ObjectBorderVisibilityConvertercan only be used OneWay.");
        }
    }
}
