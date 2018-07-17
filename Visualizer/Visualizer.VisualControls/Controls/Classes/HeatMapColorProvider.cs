using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Visualizer.VisualControls.Controls.Classes
{
    public class HeatMapColorProvider
    {
        private readonly double _minValue;
        private readonly double _maxValue;
        //private readonly List<double[]> _baseColors;

        public HeatMapColorProvider(double minValue, double maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;            
        }

        public Brush GetBrush(double value)
        {
          return    new SolidColorBrush(GetColor(value));
        }

        public Color GetColor(double value)
        {
            double red = 255;
            double green = 255;
            double blue = 255;

            if (value <= 0)
            {
                green = 255 * ((_minValue - value) / _minValue);
                blue = green;                
            }
            else
            {                               
                red = 255 * ((_maxValue-value) / _maxValue);
                blue = red;
            }

            return new Color { A = 255, R= (byte)(red), G = (byte)(green), B = (byte)(blue) };
        }
    }
}
