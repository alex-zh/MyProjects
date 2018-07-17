using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Common.Classes.Builders;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Viewer.classes
{
    /// <summary>
    /// Gets canvas height, trend part length and start, end point 
    /// All are relative to canvas coordinate
    /// </summary>
    public class VisualTrendBuilder
    {
        private readonly int _canvasHeight;        

        public VisualTrendBuilder(int canvasHeight)
        {
            _canvasHeight = canvasHeight;
        }

        public List<Shape> Build(Point start, Point end,  TrendTypes trendType)
        {
            var shapes = new List<Shape>();
        
            var brush = GetBrush(trendType);

            shapes.Add(CreateCircle(start, brush));
            shapes.Add(CreateLine(start, end, brush, 1));
            shapes.Add(CreateCircle(end, brush));
        
            return shapes;
        }

        private Brush GetBrush(TrendTypes trendType)
        {
            switch (trendType)
            {
                case TrendTypes.Up:
                    return Brushes.Green;
                case TrendTypes.Down:
                    return Brushes.Red;
                case TrendTypes.Sideward:
                    return Brushes.Blue;
            }

            return Brushes.Black;
        }

        private Shape CreateCircle(Point pt, Brush brush)
        {
            var ellipse = new Ellipse()
            {
                Stroke = brush,
                StrokeThickness = 1,
                Fill = Brushes.Blue,
                Width = 8,
                Height = 8,
            };

            Canvas.SetLeft(ellipse, pt.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, pt.Y - ellipse.Width / 2);

            return ellipse;
        }

        private Shape CreateLine(Point pt1, Point pt2, Brush lineColor, double lineThickness)
        {
            var line = new Line
            {
                Stroke = lineColor,
                StrokeThickness = lineThickness,
                X1 = pt1.X,
                Y1 = pt1.Y,
                X2 = pt2.X,
                Y2 = pt2.Y
            };

            return line;
        }
    }
}
