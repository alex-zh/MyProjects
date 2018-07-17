using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Scatterplot;

namespace RegessionAnalisys
{
    public class ScatterplotCalculator
    {
        private IEnumerable<ScatterplotPoint> _points;

        public ScatterplotCalculator(IEnumerable<ScatterplotPoint> points)
        {
            if (points.Any() == false)
            {
                throw new ArgumentException("No points!");
            }

            _points = points;
            MinX = points.Min(p => p.X);
            MaxX = points.Max(p => p.X);
            MinY = points.Min(p => p.Y);
            MaxY = points.Max(p => p.Y);
        }

        public double MinX { get; set; }

        public double MaxX { get; set; }

        public double MinY { get; set; }

        public double MaxY { get; set; }
    }
}
