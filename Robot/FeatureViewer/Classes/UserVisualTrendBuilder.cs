using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Viewer.classes
{
    /// <summary>
    /// Gets canvas height, trend part length and start, end point 
    /// All are relative to canvas coordinate
    /// </summary>
    public class UserVisualTrendBuilder
    {
        private readonly int _canvasHeight;
        private readonly double _trendPartLength;

        public UserVisualTrendBuilder(int canvasHeight, double trendPartLength)
        {
            _canvasHeight = canvasHeight;
            _trendPartLength = trendPartLength;
        }

        public List<Shape> Build(int partsCount, Point start, Point end)
        {
            var shapes = new List<Shape>();
        
            double  x1 = start.X, y1 = _canvasHeight - start.Y;
            double x2 = end.X, y2 = _canvasHeight - end.Y;

            var a = (y2 - y1) / (x2 - x1);
            var b = (y1 * x2 - y2 * x1) / (x2 - x1);

            for (int i = 0; i < partsCount; i++)
            {
                if (Math.Abs(x2 - x1) > double.Epsilon && Math.Abs(y2 - y1) > double.Epsilon)
                {
                    var startY = y1 + i * _trendPartLength * Math.Sign(y2 - y1);
                    var startX = (startY - b) / a;

                    var endY = y1 + (i + 1) * _trendPartLength * Math.Sign(y2 - y1);
                    endY = Math.Sign(y2 - y1) > 0 ? Math.Min(endY, y2) : Math.Max(endY, y2);
                    var endX = (endY - b) / a;

                    shapes.Add(CreateCircle(new Point(startX, _canvasHeight - startY), Brushes.Blue));
                    shapes.Add(CreateLine(new Point(startX, _canvasHeight - startY), new Point(endX, _canvasHeight - endY), Brushes.Red, 1));
                }
            }

            return shapes;
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
