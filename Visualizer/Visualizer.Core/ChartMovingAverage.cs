using System.Collections.Generic;
using System.Windows.Media;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Visualizer.Core
{
    public class ChartMovingAverage : MovingAverage, IChartMovingAverageLine
    {
        private readonly SolidColorBrush _color;
        private readonly int _lineThickness;
        
        public ChartMovingAverage(IEnumerable<Candle> candles, int period)
            : this(candles, period, PriceTypes.Close, Brushes.Black, 1)
        {
        }

        public ChartMovingAverage(IEnumerable<Candle> candles, int period, PriceTypes priceType, SolidColorBrush color, int lineThickness)
            : base(candles, period, priceType)
        {
            _color = color;
            _lineThickness = lineThickness;
        }
        
        public Brush Color { get { return _color; } }
        public double LineThickness { get { return _lineThickness; } }
    }
}