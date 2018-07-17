using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Classes.Builders;
using Common.Classes.General;
using Viewer.classes;
using Visualizer.VisualControls.Classes;
using TrendTypes = Common.Classes.Builders.TrendTypes;

namespace Viewer.Controls
{
    public partial class CandlesViewer
    {
        private List<Candle> _allCandles = new List<Candle>();
        private readonly List<Candle> _candles = new List<Candle>();
        private List<TrendPart> _trendParts;
        public readonly List<Shape> _trendsShapes = new List<Shape>();
        private TrendFeature _trendFeature;

        public CandlesViewer()
        {
            InitializeComponent();
        }

        public IStockChart Chart
        {
            get { return StockChart; }
        }

        public void ShowCandles(IEnumerable<Candle> candles)
        {
            _allCandles = candles.ToList();
          
            Show();
        }

        private void Show()
        {
            _candles.Clear();

            foreach (var candle in _allCandles)
            {
                var logCandle = candle.Clone();

                if (ButtonUseLogScale.IsChecked != null && ButtonUseLogScale.IsChecked.Value)
                {
                    logCandle.Open = Math.Log(logCandle.Open);
                    logCandle.Close = Math.Log(logCandle.Close);
                    logCandle.High = Math.Log(logCandle.High);
                    logCandle.Low = Math.Log(logCandle.Low);
                }

                _candles.Add(logCandle.Clone());
            }

            Chart.Init(_candles);

            Chart.Emas.Add(new ChartMovingAverage(_candles, 11, PriceTypes.Close, Brushes.Red, 1));
            Chart.Emas.Add(new ChartMovingAverage(_candles, 21, PriceTypes.Close, Brushes.DarkRed, 1));

            Chart.Draw();
        }

        private void BuildTrends(List<Candle> candles )
        {
            var minMove = new StatisticsCalculator(candles.Where(x => x.Date.Hour > 10 && x.Date.Hour < 23).Select(x => Math.Abs(x.Close - x.Open))).Median();

            var options = new TrendBuilderOptions
            {                
                AverageOneDayMove = minMove,
            };

            _trendParts =  new TrendBuilderNew(candles.ToList(), options).Build();
        }
        
        public void CustomDrawRangeFunctionForTrends(Canvas canvas, DateTime startDate, DateTime endDate)
        {
            ClearShapes();

            if (_trendParts != null)
            {
                var rangeParts = _trendParts.Where(x => _candles[x.EndIndex].Date >= startDate &&
                                                        _candles[x.StartIndex].Date <= endDate).ToList();

                if (rangeParts.Any())
                {
                    foreach (var rangePart in rangeParts)
                    {
                        if(rangePart.TrendType == TrendTypes.Sideward)
                            continue;

                        var shapes = BuildShapesForTrendPart(rangePart);

                        _trendsShapes.AddRange(shapes);
                    }
                }
            }

            DrawShapes();
        }

        public void DrawShapes()
        {
            foreach (var shape in _trendsShapes)
            {
                Chart.Canvas.Children.Add(shape);
            }
        }

        public void ClearShapes()
        {
            foreach (var shape in _trendsShapes)
            {
                Chart.Canvas.Children.Remove(shape);
            }

            _trendsShapes.Clear();
        }

        private IEnumerable<Shape> BuildShapesForTrendPart(TrendPart rangePart)
        {
            var startCandle = _candles[rangePart.StartIndex];
            var endCandle = _candles[rangePart.EndIndex];

            var visualTrendBuilder = new VisualTrendBuilder((int)Chart.ChartCalculator.CanvasHeight);

            var x1 = Chart.ChartCalculator.GetCoordinateByDate(startCandle.Date);
            var y1 = Chart.ChartCalculator.GetCoordinateByValue(startCandle.Open);

            var x2 = Chart.ChartCalculator.GetCoordinateByDate(endCandle.Date);
            var y2 = Chart.ChartCalculator.GetCoordinateByValue(endCandle.Close);

            var shapes = visualTrendBuilder.Build(new Point(x1, y1), new Point(x2, y2), rangePart.TrendType);
            return shapes;
        }

        public IEnumerable<Shape> BuildShapes(DatePrice start, DatePrice end)
        {
            var visualTrendBuilder = new VisualTrendBuilder((int)Chart.ChartCalculator.CanvasHeight);

            var x1 = Chart.ChartCalculator.GetCoordinateByDate(start.Date);
            var y1 = Chart.ChartCalculator.GetCoordinateByValue(start.Value);

            var x2 = Chart.ChartCalculator.GetCoordinateByDate(end.Date);
            var y2 = Chart.ChartCalculator.GetCoordinateByValue(end.Value);

            var shapes = visualTrendBuilder.Build(new Point(x1, y1), new Point(x2, y2), TrendTypes.Sideward);

            return shapes;
        }

        private void ButtonUseLogScale_OnClick(object sender, RoutedEventArgs e)
        {
            Show();
        }

        public void InitCustomDrawRangeFunction(Action<Canvas, DateTime, DateTime> customDrawRangeFunction)
        {
            Chart.InitCustomDrawRangeFunction(customDrawRangeFunction);
        }
    }
}
