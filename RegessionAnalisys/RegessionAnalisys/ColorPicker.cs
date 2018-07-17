using System;
using System.Drawing;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace RegessionAnalisys
{
    public class ColorPicker
    {
        public Color Pick(int category)
        {
            byte r = 0, g = 0, b = 0;

            switch (Math.Abs(category))
            {
                case 0:
                    r = 255;
                    break;
                case 1:
                    g = 255;
                    break;
                case 2:
                    b = 255;
                    break;
                case 3:
                    r = 255;
                    g = 255;
                    break;
                case 4:
                    g = 255;
                    b = 255;
                    break;
                case 5:
                    r = 255;
                    b = 255;
                    break;               
                default:
                    throw new NotSupportedException("Group '" + category + "' is not supported");
            }

            return Color.FromArgb(255, r, g, b);            
        }


        public Color RevertColor(Color color)
        {
            const int rgbmax = 255;

            var revertedColor = Color.FromArgb(255, (byte)(rgbmax - color.R), (byte)(rgbmax - color.G), (byte)(rgbmax - color.B));

            return revertedColor;
        }

        public string GetName(Color color)
        {
            foreach (var colorValue in Enum.GetValues(typeof(KnownColor)))
            {
                var c = System.Drawing.Color.FromKnownColor((KnownColor)colorValue);
                if (color.R == c.R && color.G == c.G && color.B == c.B)
                {
                    return ((KnownColor) colorValue).ToString();
                }
            }

            return "Unknown color";
        }
    }
}
