using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Common;

namespace Visualizer.Core
{
    public class ChartLine: IChartLine
    {
        private readonly SolidColorBrush _color;
        private readonly int _lineThickness;        
        private readonly List<DatePrice> _values;
        
        public ChartLine(IEnumerable<DatePrice> values,  SolidColorBrush color, int lineThickness)
        {
            _values = values.ToList();            
            _color = color;
            _lineThickness = lineThickness;
        }

        public DatePrice this[int index]
        {
            get { return _values[index]; }
        }
        
        public Brush Color { get { return _color; } }
        public double LineThickness { get { return _lineThickness; } }
    }
}