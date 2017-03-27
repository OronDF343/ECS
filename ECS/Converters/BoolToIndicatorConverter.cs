using System;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class BoolToIndicatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as bool? == true ? "*" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }
    }
}