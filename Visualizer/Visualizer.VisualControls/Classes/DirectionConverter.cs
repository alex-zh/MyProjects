using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Common;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Visualizer.VisualControls.Classes
{
    [ValueConversion(typeof(int), typeof(string))]
    public class RobotNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var robotId = (int)value;

            if (Enum.IsDefined(typeof(RobotNames), robotId))
            {
                return Enum.GetName(typeof (RobotNames), robotId);
            }

            return "General";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Direction), typeof(string))]
    public class DirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = (value.GetType() == typeof (double)) ? (double)value: (int)value;

            return size > 0 ? "Buy" : "Sell";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}