using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace ECS.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var myEnum = value as Enum;
            var description = GetEnumDescription(myEnum);
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static string GetEnumDescription(Enum enumObj)
        {
            if (enumObj == null) return null;

            var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            var attribArray = fieldInfo?.GetCustomAttributes(false);

            if (attribArray == null || attribArray.Length == 0) return enumObj.ToString();

            var attrib = attribArray[0] as DescriptionAttribute;
            return attrib?.Description ?? enumObj.ToString();
        }
    }
}
