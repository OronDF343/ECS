using System;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class LinkOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as double?) / 2.0 + (parameter as double?) ?? 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as double?) * 2.0 - (parameter as double?) ?? 0;
        }
    }
}
