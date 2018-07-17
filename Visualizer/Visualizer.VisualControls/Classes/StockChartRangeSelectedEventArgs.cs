using Visualizer.VisualControls.Controls;

namespace Visualizer.VisualControls.Classes
{
    public class StockChartRangeSelectedEventArgs
    {
        public StockChartRange Range{ get; set; }

        public StockChartRangeSelectedEventArgs(StockChartRange range)
        {
            Range = range;
        }
    }
}