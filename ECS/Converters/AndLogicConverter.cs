using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ECS.Converters
{
    public class AndLogicConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values?.Where(v => v != null).Cast<bool>().Aggregate(true, (current, val) => current && val);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
