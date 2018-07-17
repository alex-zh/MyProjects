using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Classes;
using Common.Classes.Builders;
using Common.Classes.General;
using Visualizer.VisualControls.Classes;
using TrendTypes = Common.Classes.Builders.TrendTypes;

namespace FeatureViewer.Classes
{
    public class SignalBuilder
    {
        private readonly IStockChart _chart;

        public SignalBuilder(IStockChart chart)
        {
            _chart = chart;
        }

        public Shape BuildCircle(Candle candle, SignalTypes signalType)
        {
            double x = _chart.ChartCalculator.GetCoordinateByDate(candle.Date);
            double y = _chart.ChartCalculator.GetCoordinateByValue(signalType == SignalTypes.Sell ? candle.High : candle.Low);

            var brushColor = GetSignalColor(signalType);

            var plg = new Polygon { Stroke = brushColor, 
                StrokeThickness = 1, 
                Fill = brushColor, 
                DataContext = signalType
            };

            if (signalType == SignalTypes.Buy)
            {
                plg.Points.Add(new Point(x - 5, y + 13));
                plg.Points.Add(new Point(x, y + 3));
                plg.Points.Add(new Point(x + 5, y + 13));
                plg.Points.Add(new Point(x - 5, y + 13));
            }
            else if (signalType  == SignalTypes.Sell)
            {
                plg.Points.Add(new Point(x - 5, y + 3));
                plg.Points.Add(new Point(x, y + 13));
                plg.Points.Add(new Point(x + 5, y + 3));
                plg.Points.Add(new Point(x - 5, y + 3));
            }

            return plg;
        }

        public IEnumerable<Shape> BuildTrendLine(Candle startCandle, Candle endCandle)
        {

            var visualTrendBuilder = new VisualTrendBuilder((int)_chart.ChartCalculator.CanvasHeight);

            var x1 = _chart.ChartCalculator.GetCoordinateByDate(startCandle.Date);
            var y1 = _chart.ChartCalculator.GetCoordinateByValue(startCandle.Get(PriceTypes.Mean));

            var x2 = _chart.ChartCalculator.GetCoordinateByDate(endCandle.Date);
            var y2 = _chart.ChartCalculator.GetCoordinateByValue(endCandle.Get(PriceTypes.Mean));

            var shapes = visualTrendBuilder.Build(new Point(x1, y1), new Point(x2, y2), TrendTypes.Sideward);
            return shapes;
        }

        public IEnumerable<Shape> BuildHorizontalRange(int startIndex, int endIndex)
        {
            var workingCandles = _chart.ChartCalculator.Candles.TakeRange(startIndex, endIndex).ToList();

            var lowLevel = workingCandles.Min(x => x.Close);
            var highLevel = workingCandles.Max(x => x.Close);
            var rangeSwing = highLevel - lowLevel;

            var yValue = lowLevel + rangeSwing/2;

            var lowY = _chart.ChartCalculator.GetCoordinateByValue(lowLevel);
            var highY = _chart.ChartCalculator.GetCoordinateByValue(highLevel);

            var visualTrendBuilder = new VisualTrendBuilder((int)_chart.ChartCalculator.CanvasHeight);

            var x1 = _chart.ChartCalculator.GetCoordinateByIndex(startIndex);
        
            var x2 = _chart.ChartCalculator.GetCoordinateByIndex(endIndex);

            var result = new List<Shape>();

            var lowLevelShapes = visualTrendBuilder.Build(new Point(x1, lowY), new Point(x2, lowY), TrendTypes.Sideward);
            result.AddRange(lowLevelShapes);
            var highLevelShapes = visualTrendBuilder.Build(new Point(x1, highY), new Point(x2, highY), TrendTypes.Sideward);
            result.AddRange(highLevelShapes);

            return result;
        }


        private static Brush GetSignalColor(SignalTypes signal)
        {
            switch (signal)
            {
                case SignalTypes.Buy:
                    return Brushes.Green;
                case SignalTypes.Sell:
                    return  Brushes.Red;
            }

            return Brushes.Black;
        }

        public FrameworkElement BuildCircle(Candle candle, Brush brush)
        {          
            var brushColor = brush;

            var ellipse = new Ellipse
            {
                Stroke = brushColor,
                StrokeThickness = 1,
                Fill = brushColor,
                Width = 10,
                Height = 10,                
            };

            return ellipse;
        }     
    }
}