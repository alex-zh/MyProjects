using System.Collections.Generic;
using System.Linq;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls.Classess
{
    public class PlainChartCalculator
    {
        private readonly List<PlainChartPoint> _points;

        public PlainChartCalculator(List<PlainChartPoint> points)
        {
            if (points.Any() == false) return;

            _points = points;

            MinX = 0;
            MaxX = points.Count - 1;

            MinY = points.Min(p => p.Value);
            MaxY = points.Max(p => p.Value);

        }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
    }
}