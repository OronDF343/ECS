using System;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class DoubleRoundConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as double? ?? 0.0;
            if (double.IsNaN(v)) v = 0.0;
            return v.ToString("#.###");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
