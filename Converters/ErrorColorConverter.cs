using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Nanpla.Converters
{
    public class ErrorColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isError && isError)
            {
                return Colors.Red;
            }
            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
