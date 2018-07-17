using System.Windows.Media;

namespace Visualizer.VisualControls.Classes
{
    public class BivariateChartPoint
    {
        public int Index { get; set; }

        public object Data { get; set; }

        public double FirstValue { get; set; }
        public double SecondValue { get; set; }

        public SolidColorBrush Brush { get; set; }
        public int Radius { get; set; }
    }
}