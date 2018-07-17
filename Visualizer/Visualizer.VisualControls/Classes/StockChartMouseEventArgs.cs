using System.Windows;

namespace Visualizer.VisualControls.Classes
{
    public class StockChartMouseEventArgs
    {
        public Point Point { get; set; }

        public StockChartMouseEventArgs(Point point)
        {
            Point = point;
        }
    }
}