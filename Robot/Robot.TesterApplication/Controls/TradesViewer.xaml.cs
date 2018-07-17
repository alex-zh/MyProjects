using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.CandlePattern;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.StrategyTester;
using Common.Finders;
using Robot.TesterApplication.Classes;
using Visualizer.VisualControls.Classes;
using Visualizer.VisualControls.Controls;

namespace Robot.TesterApplication.Controls
{   
    public partial class TradesViewer
    {
        private IEnumerable<Candle> _candles;
        private IEnumerable<TradeSignal> _signals;
        private readonly TradeSignalBuilder _tradeSignalBuilder;
        private TradeSignal _currentTradeSignal, _testTradeSignal;
        private readonly List<Line> _tradeSignalLines = new List<Line>();
        
        public TradesViewer()
        {
            InitializeComponent();
            Chart.MouseMoved += Chart_MouseMoved;
            Chart.RangeSelected += Chart_RangeSelected;
            _tradeSignalBuilder = new TradeSignalBuilder(Chart);
        }

      
        public IStockChart Chart
        {
            get { return StockChart; }
        }

        public void ShowCandles(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

            Chart.Init(_candles);

           // Chart.Emas.Add(new ChartMovingAverage(_candles, 3, PriceTypes.Mean, Brushes.Green, 2));
            Chart.Emas.Add(new ChartMovingAverage(_candles, 11, PriceTypes.Mean, Brushes.Red, 1));
            Chart.Emas.Add(new ChartMovingAverage(_candles, 21, PriceTypes.Mean, Brushes.Maroon, 1));

            Chart.InitGetCandleDrawOptionsFunction(GetCandleDrawOptions);

            Chart.Draw();
        }

        public void ShowTrades(IEnumerable<Candle> candles, IEnumerable<TradeSignal> signals)
        {
            _candles = candles.ToList();
            _signals = signals;
            
            Chart.Init(_candles);

            Chart.Emas.Add(new ChartMovingAverage(_candles, 11, PriceTypes.Mean, Brushes.Red, 1));
            Chart.Emas.Add(new ChartMovingAverage(_candles, 21, PriceTypes.Mean, Brushes.Maroon, 1));

            Chart.InitCustomDrawFunction(Draw);
            Chart.InitGetCandleDrawOptionsFunction(GetCandleDrawOptions);

            Chart.Draw();          
        }       

        public void Draw(Canvas canvas, Candle candle)
        {
            var signals = GetSignalsForDate(candle.Date).ToList();

            for(int i=0 ;i < signals.Count(); i++)
            {
                var signal = signals[i];
                var plg = _tradeSignalBuilder.BuildTradeSignal(candle, signal, i);
                plg.DataContext = signal;

                canvas.Children.Add(plg);
            }        
        }

        private void DrawIndicator(Canvas canvas, Candle candle)
        {
            throw new NotImplementedException();
        }

        private CandleDrawOptions GetCandleDrawOptions(Candle candle)
        {
            var result = new CandleDrawOptions();
            return result;

            var index = Chart.ChartCalculator.GetIndexByDate(candle.Date);

            if (index < 32)
            {
                return result;
            }

            var allCandles = Chart.ChartCalculator.Candles.ToList();
            var atrsFinder = new AtrStatisticsFinder(allCandles.TakeRange(Math.Max(0, index - 1000), index).ToList(), 16);
            var atrStatistics = new StatisticsCalculator(atrsFinder.Find()).GetParameters();

            var atr = new SimpleAtr(allCandles.TakeRange(index - 16, index).ToArray()).Calculate();

            if (atr > atrStatistics.FirstQuartile && atr<atrStatistics.Median)
            {
                result.BrushColor = Brushes.LightPink;
                result.StrokeThickness = 2;    
            }
            else if (atr > atrStatistics.Median && atr < atrStatistics.ThirdQuartile)
            {
                result.BrushColor = Brushes.LightCoral;
                result.StrokeThickness = 2;
            }
            else if ( atr > atrStatistics.ThirdQuartile)
            {
                result.BrushColor = Brushes.Red;
                result.StrokeThickness = 2;
            }

            
            //var candlePatternsProvider = new CandlePatternsProvider(atr);
            //var patternsFinder = new CandlePatternFinder();

            //foreach (var upPattern in candlePatternsProvider.GetAllForUp())
            //{
            //    var testCandles = Chart.ChartCalculator.Candles.TakeRange(index - upPattern.PatternCandlesCount + 1, index).ToArray();

            //    if (patternsFinder.DoesMatch(testCandles, upPattern))
            //    {
            //        result.BrushColor = Brushes.Green;
            //        result.StrokeThickness = 2;

            //        return result;
            //    }
            //}

            return result;
        }

        /// <summary>
        /// Для свечи может быть два сигнала, один на закрытие текущей позиции, второй на открытие новой позиции
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private IEnumerable<TradeSignal> GetSignalsForDate(DateTime date)
        {
            return _signals.Where(x => x.Candle.Date == date);
        }

        void Chart_MouseMoved(object sender, StockChartMouseEventArgs e)
        {
            _testTradeSignal = null;

            VisualTreeHelper.HitTest(Chart.Canvas, null, MouseInSingalAreaHitTest, new PointHitTestParameters(e.Point));

            if (_testTradeSignal != _currentTradeSignal)
            {                
                _currentTradeSignal = _testTradeSignal;

                DrawTradeSignalLines();
            }
        }

        private void Chart_RangeSelected(object sender, StockChartRangeSelectedEventArgs e)
        {
            var range = _candles.GetRange(e.Range.Start, e.Range.End);

            Clipboard.SetText(range.ToText());
        }


        private void DrawTradeSignalLines()
        {
            foreach (var line in _tradeSignalLines)
            {
                Chart.Canvas.Children.Remove(line);
            }

            _tradeSignalLines.Clear();

            if (_currentTradeSignal == null) return;

            _tradeSignalLines.AddRange(_tradeSignalBuilder.BuildLines(_currentTradeSignal));

            foreach (var line in _tradeSignalLines)
            {
                Chart.Canvas.Children.Add(line);
            }
        }

        private HitTestResultBehavior MouseInSingalAreaHitTest(HitTestResult result)
        {
            var hit = result.VisualHit as Polygon;

            if (hit != null)
            {
                var signal = ((Polygon) (result.VisualHit)).DataContext as TradeSignal;

                if (signal != null)
                {
                    _testTradeSignal = signal;
                    return HitTestResultBehavior.Stop;
                }
            }

            return HitTestResultBehavior.Continue;
        }

        public void GoTo(int selectedIndex)
        {
            Chart.GoTo(selectedIndex);
        }
    }
}
