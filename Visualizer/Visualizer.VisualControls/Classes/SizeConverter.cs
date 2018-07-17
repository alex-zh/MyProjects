using System;
using System.Globalization;
using System.Windows.Data;

namespace Visualizer.VisualControls.Classes
{
    [ValueConversion(typeof(int), typeof(string))]
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (double)value;

            return Math.Abs(size).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
