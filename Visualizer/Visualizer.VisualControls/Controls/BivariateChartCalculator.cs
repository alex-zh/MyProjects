using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls
{
    public class BivariateChartCalculator
    {
        private readonly List<BivariateChartPoint> _points;

        public BivariateChartCalculator(List<BivariateChartPoint> points)
        {
            if(points.Any() == false) return;

            _points = points;   

            MinX = points.Min(p => p.FirstValue);
            MaxX =  points.Max(p => p.FirstValue);

            MinY = points.Min(p => p.SecondValue);
            MaxY = points.Max(p => p.SecondValue);

        }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
    }
}