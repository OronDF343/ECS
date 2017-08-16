using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ECS.Converters
{
    public class CurrentToBrushConverter : IValueConverter
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public Color MinColor { get; set; }
        public Color MidColor { get; set; }
        public Color MaxColor { get; set; }
        public Color NullColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = Math.Abs(value as double? ?? 0.0);
            if (double.IsNaN(v)) v = 0.0;
            if (v < 1e-13) return new SolidColorBrush(NullColor);
            v = Math.Max(v, MinValue);
            v = Math.Min(v, MaxValue);
            var factor = 2 * (v - MinValue) / (MaxValue - MinValue) ;
            return new SolidColorBrush(new Color
            {
                A = 255,
                R = GenColorValue(MinColor.R, MidColor.R, MaxColor.R, factor),
                G = GenColorValue(MinColor.G, MidColor.G, MaxColor.G, factor),
                B = GenColorValue(MinColor.B, MidColor.B, MaxColor.B, factor)
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private byte GenColorValue(byte min, byte mid, byte max, double factor)
        {
            return factor > 1.0 ? (byte)(mid + (max - mid) * (factor - 1.0)) : (byte)(min + (mid - min) * factor);
        }
    }
}
