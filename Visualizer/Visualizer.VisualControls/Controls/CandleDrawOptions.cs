using System.Windows.Media;

namespace Visualizer.VisualControls.Controls
{
    public class CandleDrawOptions
    {
        public CandleDrawOptions()
        {
            StrokeThickness = 1;
            BrushColor = Brushes.Black;            
        }

        public int StrokeThickness { get; set; }
        public SolidColorBrush BrushColor { get; set; }
    }
}