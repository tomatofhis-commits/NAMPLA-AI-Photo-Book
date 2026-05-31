using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Nanpla.Converters
{
    public class NoteVisibleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<int> notes && parameter != null)
            {
                if (int.TryParse(parameter.ToString(), out int num))
                {
                    if (notes.Contains(num))
                    {
                        return num.ToString();
                    }
                }
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
