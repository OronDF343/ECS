using System;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class ComponentOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var src = values?[0] as double? ?? 0;
            var dest = values?[1] as double? ?? 0;
            var offset = parameter as double? ?? 0;
            return dest - src + offset;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
