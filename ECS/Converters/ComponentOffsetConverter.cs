using System;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class ComponentOffsetConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var offset = values[0] as double? ?? 0;
            // TODO: check side of connection and apply correct offsets
            return offset + 10;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}