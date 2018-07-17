using System.Collections.Generic;
using System.Windows.Media;
using Common;

namespace Visualizer.VisualControls.Classes
{
    public class ChartLine
    {
        private readonly List<DatePrice> _values;

        public ChartLine(List<DatePrice> values)
            : this(values, Brushes.Black, 1)
        {
        }

        public ChartLine(List<DatePrice> values, SolidColorBrush brush, int lineThickness)
        {            
            _values = values;

            Brush = brush;
            LineThickness = lineThickness;
        }

        public Brush Brush { get; set; }    
        
        public double LineThickness { get;set;}

        public DatePrice this[int index]
        {
            get { return _values[index]; }
        }

        public int Count
        {
            get { return _values.Count; }
        }        
    }
}