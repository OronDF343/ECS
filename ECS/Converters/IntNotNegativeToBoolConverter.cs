using System;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class IntNotNegativeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as int? ?? 0) > -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
