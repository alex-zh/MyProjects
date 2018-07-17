using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using Common.Classes.General;

namespace Visualizer.VisualControls.Classes
{
    public class TrendLine
    {
        private List<Shape> _shapes = new List<Shape>();

        public TrendTypes TrendType { get; set; }
        public Tuple<Candle, double> Start { get; set; }
        public Tuple<Candle, double>  End { get; set; }

        public List<Shape> Shapes
        {
            get { return _shapes; }
        }

        public TrendLine Clone()
        {
            var result = new TrendLine {Start = Start, End = End, TrendType = TrendType};

            foreach (var shape in Shapes)
            {
                result.Shapes.Add(shape);
            }

            return result;
        }
    }
}