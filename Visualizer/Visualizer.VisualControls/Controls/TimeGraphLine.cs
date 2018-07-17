using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls
{
    public class TimeGraphLine
    {
        private readonly List<PlainChartPoint> _points;
        private readonly AxisSides _axisSide;
        private Color _color;
        private readonly string _legend;

        public TimeGraphLine(List<PlainChartPoint> points, AxisSides axisSide, Color color, string legend)
        {
            _points = points;
            _axisSide = axisSide;
            _color = color;
            _legend = legend;
        }

        public Color Color {
            get { return _color; }
        }

        public string Legend
        {
            get { return _legend; }
        }

        public List<PlainChartPoint> Points
        {
            get { return _points; }
        }
    }
}