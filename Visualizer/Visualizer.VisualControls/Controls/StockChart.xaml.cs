using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls
{
    public partial class StockChart : IStockChart
    {
        public event EventHandler<StockChartMouseEventArgs> MouseMoved;
        public new event EventHandler<StockChartMouseEventArgs> MouseUp;
        public new event EventHandler<StockChartMouseEventArgs> MouseDown;
        public event EventHandler<StockChartRangeSelectedEventArgs> RangeSelected;

        private Action<Canvas, Candle> _customDrawFunction;
        private Action<Canvas, DateTime, DateTime> _customDrawRangeFunction;
        private Func<Candle, CandleDrawOptions> _getCandleDrawOptions;

        private ChartCalculator _chartCalculator;       

        public static int CandleHalfWidth = 3;
        public static int CandleOffset = 3;
        private readonly List<ChartLine> _chartLinesList = new List<ChartLine>();
        private readonly List<ChartMovingAverage> _emaList = new List<ChartMovingAverage>();

        public StockChart()
        {
            InitializeComponent();
            canvasChart.PreviewMouseUp += CanvasChartOnPreviewMouseUp;
            canvasChart.PreviewMouseDown += CanvasChartOnPreviewMouseDown;
            canvasChart.MouseLeftButtonDown += CanvasChart_MouseLeftButtonDown;
            canvasChart.MouseLeftButtonUp += CanvasChart_MouseLeftButtonUp;
            gridChart.SizeChanged += gridChart_SizeChanged;
            scrollBarHorizontal.ValueChanged += scrollBarHorizontal_ValueChanged;

            _customDrawFunction = (canvas, candle) => { };
            _customDrawRangeFunction = (canvas, time, arg3) => { };

            ViewPortDataHolder = new ViewPortDataHolder();
        }


        public Canvas Canvas
        {
            get { return canvasChart; }
        }

        public List<ChartMovingAverage> Emas
        {
            get { return _emaList; }
        }

        public List<ChartLine> ChartLines
        {
            get { return _chartLinesList; }
        }

        public ViewPortDataHolder ViewPortDataHolder
        {
            get;
            private set;
        }

        public ChartCalculator ChartCalculator
        {
            get
            {
                if (_chartCalculator != null)
                {
                    _chartCalculator.InitMinMaxValues(ViewPortDataHolder.ViewPortMinValue,
                                                       ViewPortDataHolder.ViewPortMaxValue,
                                                       ViewPortDataHolder.ViewPortStartDate,
                                                       ViewPortDataHolder.ViewPortEndDate);
                }
                return _chartCalculator;
            }
            set { _chartCalculator = value; }
        }

        public void InitCustomDrawFunction(Action<Canvas, Candle> customDrawFunction)
        {
            _customDrawFunction = customDrawFunction;
        }

        public void InitCustomDrawRangeFunction(Action<Canvas, DateTime, DateTime> customDrawRangeFunction)
        {
            _customDrawRangeFunction = customDrawRangeFunction;
        }

        public void InitGetCandleDrawOptionsFunction(Func<Candle, CandleDrawOptions> getCandleDrawOptions)
        {
            _getCandleDrawOptions = getCandleDrawOptions;
        }

        public bool IsReady
        {
            get
            {
                return canvasChart.IsVisible &&
                       ChartCalculator != null &&
                       ChartCalculator.Candles != null &&
                       ChartCalculator.Candles.Any();
            }
        }

        private void gridChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PrepareAndDrawChart();
        }

        private void PrepareAndDrawChart()
        {
            if (IsReady == false)
            {
                return;
            }

            InitChart();            

            var newValue = scrollBarHorizontal.Track.Value;

            SynchronizeScrollPosition(newValue);

            ViewPortDataHolder.Init(newValue,
                                    newValue + scrollBarHorizontal.Track.ViewportSize,
                                    ChartCalculator);

            axisVertical.Init(ViewPortDataHolder.ViewPortMinValue, ViewPortDataHolder.ViewPortMaxValue);
            axisHorizontal.Init(ChartCalculator, GroupingInterval.Hour);

            //customIndicatorChart.Init(ChartCalculator.Candles.ToList());

            Draw();
        }

        private void scrollBarHorizontal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;

            SynchronizeScrollPosition(newValue);

            ViewPortDataHolder.Init(newValue,
                                    newValue + scrollBarHorizontal.Track.ViewportSize,
                                    ChartCalculator);

            axisVertical.Init(ViewPortDataHolder.ViewPortMinValue, ViewPortDataHolder.ViewPortMaxValue);

            Draw();
        }

        private void InitChart()
        {
            if (ChartCalculator != null)
            {
                canvasChart.Width = ChartCalculator.CalculateCanvasWidth();
            }
            else
            {
                canvasChart.Width = gridChart.ColumnDefinitions[0].ActualWidth;
            }

            canvasChart.Height = gridChart.RowDefinitions[0].ActualHeight;

            ChartCalculator.CanvasWidth = canvasChart.Width;
            ChartCalculator.CanvasHeight = canvasChart.Height;

            axisHorizontal.AxisWidth = canvasChart.Width;
            axisVertical.AxisHeight = canvasChart.Height;

            scrollBarHorizontal.Minimum = 0;
            scrollBarHorizontal.Maximum = canvasChart.Width - gridChart.ColumnDefinitions[0].ActualWidth; //scrollViewerCanvasChart.ScrollableWidth;
            scrollBarHorizontal.ViewportSize = gridChart.ColumnDefinitions[0].ActualWidth;

            scrollBarHorizontal.SmallChange = CandleHalfWidth * 2;
            scrollBarHorizontal.LargeChange = 10 * CandleHalfWidth * 2;
        }

        private void SynchronizeScrollPosition(double newValue)
        {
            scrollViewerCanvasChart.ScrollToHorizontalOffset(newValue);
            scrollViewerDateTimeAxis.ScrollToHorizontalOffset(newValue);
            //customIndicatorChart.ScrollToHorizontalOffset(newValue);
        }

        private void DrawVerticalGridLines()
        {
            var minY = ViewPortDataHolder.ViewPortMinValue;
            var maxY = ViewPortDataHolder.ViewPortMaxValue;

            var minX = ViewPortDataHolder.ViewPortStartDate;
            var maxX = ViewPortDataHolder.ViewPortEndDate;

            var ticksCalc = new AxisTicksCalculator(minY, maxY);

            foreach (double dy in ticksCalc.GetTicks())
            {
                var gridline = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.3,
                    StrokeDashArray = new DoubleCollection(new double[2] { 3, 9 }),
                    X1 = ChartCalculator.GetCoordinateByDate(minX),
                    X2 = ChartCalculator.GetCoordinateByDate(maxX),
                    Y1 = ChartCalculator.GetCoordinateByValue(dy, minY, maxY)
                };

                gridline.Y2 = gridline.Y1;

                canvasChart.Children.Add(gridline);
            }
        }

        public void Init(IEnumerable<Candle> candles)
        {
            _emaList.Clear();

            ChartCalculator = new ChartCalculator(candles,
                                                  CandleHalfWidth,
                                                  CandleOffset,
                                                  canvasChart.Width,
                                                  canvasChart.Height);
            PrepareAndDrawChart();
        }

        public void Draw()
        {
            Visibility = Visibility.Visible;

            canvasChart.Children.Clear();

            DrawVerticalGridLines();

            double minY = ViewPortDataHolder.ViewPortMinValue;
            double maxY = ViewPortDataHolder.ViewPortMaxValue;

            var viewPortCandles = ChartCalculator.Candles.Where(x => x.Date >= ViewPortDataHolder.ViewPortStartDate && 
                                                                     x.Date < ViewPortDataHolder.ViewPortEndDate).ToList();

            foreach (var candle in viewPortCandles)
            {
                
                DrawCandle(candle, minY, maxY);
                DrawEmas(candle, minY, maxY);

                if (_customDrawFunction != null)
                {
                    _customDrawFunction(canvasChart, candle);
                }
            }
           
            _customDrawRangeFunction(canvasChart, ViewPortDataHolder.ViewPortStartDate, ViewPortDataHolder.ViewPortEndDate);
                        
            //customIndicatorChart.DrawRange(viewPortCandles);
        }

        public void GoTo(int index)
        {
            if (ChartCalculator != null && ChartCalculator.Candles.Any() &&
                index >= 0 && index < ChartCalculator.Candles.Count())
            {
                var coordinate = ChartCalculator.GetCoordinateByIndex(index) - scrollBarHorizontal.ViewportSize / 2;
                scrollBarHorizontal.Value = coordinate;
            }
        }

        private void DrawEmas(Candle candle, double minY, double maxY)
        {
            var index = ChartCalculator.IndexOf(candle);

            var x1 = ChartCalculator.GetCoordinateByIndex(index - 1);
            var x2 = ChartCalculator.GetCoordinateByIndex(index);

            foreach (var ema in _emaList)
            {
                if (index <= ema.Period) continue;

                var previousValue = ema[index - 1].Value;
                var currentValue = ema[index].Value;

                var y1 = ChartCalculator.GetCoordinateByValue(previousValue, minY, maxY);
                var y2 = ChartCalculator.GetCoordinateByValue(currentValue, minY, maxY);

                DrawLine(canvasChart, x1, y1, x2, y2, ema.Color, ema.LineThickness);
            }

            foreach (var chartLine in _chartLinesList)
            {
                var previousValue = chartLine[index - 1].Value;
                var currentValue = chartLine[index].Value;

                var y1 = ChartCalculator.GetCoordinateByValue(previousValue, minY, maxY);
                var y2 = ChartCalculator.GetCoordinateByValue(currentValue, minY, maxY);

                DrawLine(canvasChart, x1, y1, x2, y2, chartLine.Brush, chartLine.LineThickness);
            }
        }

        private void DrawCandle(Candle candle, double minY, double maxY)
        {
            var x = ChartCalculator.GetCoordinateByDate(candle.Date);

            var openValue = ChartCalculator.GetCoordinateByValue(candle.Open, minY, maxY);
            var closeValue = ChartCalculator.GetCoordinateByValue(candle.Close, minY, maxY);
            var lowValue = ChartCalculator.GetCoordinateByValue(candle.Low, minY, maxY);
            var highValue = ChartCalculator.GetCoordinateByValue(candle.High, minY, maxY);

            var ptOpen1 = new Point(x - CandleHalfWidth, openValue);
            var ptOpen2 = new Point(x + CandleHalfWidth, openValue);

            var ptClose1 = new Point(x + CandleHalfWidth, closeValue);
            var ptClose2 = new Point(x - CandleHalfWidth, closeValue);

            DrawLine(canvasChart, new Point(x, lowValue), new Point(x, highValue), Brushes.Black, 1);

            var brushColor = Brushes.Black;
            var strokeThickness = 1;

            if (_getCandleDrawOptions != null)
            {
                var options = _getCandleDrawOptions(candle);

                brushColor = options.BrushColor;
                strokeThickness = options.StrokeThickness;
            }

            var plg = new Polygon { Stroke = brushColor, StrokeThickness = strokeThickness };

            Brush fillColor = Brushes.Black;

            if (candle.Close > candle.Open)
            {
                fillColor = Brushes.White;
            }

            plg.Fill = fillColor;
            plg.Points.Add(ptOpen1);
            plg.Points.Add(ptOpen2);
            plg.Points.Add(ptClose1);
            plg.Points.Add(ptClose2);

            canvasChart.Children.Add(plg);
        }

        private void DrawLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush lineColor, double lineThickness)
        {
            var line = new Line
            {
                Stroke = lineColor,
                StrokeThickness = lineThickness,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            canvas.Children.Add(line);
        }

        private void DrawLine(Canvas canvas, Point pt1, Point pt2, Brush lineColor, double lineThickness)
        {
            Line line = new Line();
            line.Stroke = lineColor;
            line.StrokeThickness = lineThickness;
            line.X1 = pt1.X;
            line.Y1 = pt1.Y;
            line.X2 = pt2.X;
            line.Y2 = pt2.Y;
            canvas.Children.Add(line);
        }

        Line crossVertical;
        Line crossHorizontal;

        public void canvasChartMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(canvasChart);

            Task.Run(() =>
            {
                Thread.Sleep(50);

                canvasChart.Dispatcher.Invoke(() =>
                {
                    // HighlightRange(point);

                    ShowInfoBox(point.X, point.Y);

                    if (MouseMoved != null)
                    {
                        MouseMoved(this, new StockChartMouseEventArgs(point));
                    }
                });
            });
        }

        private void HighlightRange(Point point)
        {
            if (_chartCalculator == null) return;

            var currentCandle = _chartCalculator.GetCandleByCoordinate(point.X);
            if (currentCandle != null && _chartRange.Start != null && currentCandle != _chartRange.Start)
            {
                var startCoordinate = _chartCalculator.GetCoordinateByDate(_chartRange.Start.Date);
                var currentCoordinate = _chartCalculator.GetCoordinateByDate(currentCandle.Date);

                var rect = new Rectangle
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1,
                    Fill = Brushes.DodgerBlue,
                    Opacity = 0.85,
                    //OpacityMask  = new LinearGradientBrush(new GradientStopCollection(new[]
                    //{
                    //    new GradientStop() {Color = Colors.DodgerBlue, Offset = 0.1},
                    //    new GradientStop() {Color = Colors.Transparent, Offset = 1}
                    //})),

                    Width = Math.Abs(startCoordinate - currentCoordinate),
                    Height = Math.Abs(_chartCalculator.GetCoordinateForMaxValue() - _chartCalculator.GetCoordinateForMinValue())
                };

                Canvas.SetLeft(rect, Math.Min(startCoordinate, currentCoordinate));
                Canvas.SetTop(rect, _chartCalculator.GetCoordinateForMaxValue());
                canvasChart.Children.Add(rect);
            }
        }

        private void ShowInfoBox(double x, double y)
        {
            if (double.IsNaN(canvasChart.Width) || double.IsNaN(canvasChart.Height))
                return;

            if (crossVertical != null && canvasChart.Children.Contains(crossVertical))
            {
                canvasChart.Children.Remove(crossVertical);
            }

            if (crossHorizontal != null && canvasChart.Children.Contains(crossHorizontal))
            {
                canvasChart.Children.Remove(crossHorizontal);
            }

            crossVertical = new Line
            {
                Stroke = Brushes.DarkRed,
                StrokeThickness = 0.5,
                X1 = x,
                X2 = x,
                Y1 = 0,
                Y2 = canvasChart.Height
            };

            canvasChart.Children.Add(crossVertical);

            crossHorizontal = new Line
            {
                Stroke = Brushes.DarkRed,
                StrokeThickness = 0.5,
                X1 = 0,
                X2 = canvasChart.Width,
                Y1 = y,
                Y2 = y
            };

            canvasChart.Children.Add(crossHorizontal);

            var candle = ChartCalculator.GetCandleByCoordinate(x);

            double minY = ViewPortDataHolder.ViewPortMinValue;
            double maxY = ViewPortDataHolder.ViewPortMaxValue;

            var openValue = ChartCalculator.GetCoordinateByValue(candle.Open, minY, maxY);
            var closeValue = ChartCalculator.GetCoordinateByValue(candle.Close, minY, maxY);

            var coordinateX = ChartCalculator.GetCoordinateByDate(candle.Date);

            cursorInfoBox.Visibility = Visibility.Visible;
            cursorInfoBox.Value = ChartCalculator.GetValueByCoordinate(y, minY, maxY).ToString("####.000");

            if (y >= Math.Min(closeValue, openValue) && y <= Math.Max(closeValue, openValue) &&
                x >= coordinateX - CandleHalfWidth && x <= coordinateX + CandleHalfWidth)
            {
                var candleIndex = ChartCalculator.Candles.ToList().IndexOf(candle);

                infoBox.Visibility = Visibility.Visible;
                infoBox.CandleDate = candle.Date.ToString();
                infoBox.CandleOpen = candle.Open.ToString("#.000");
                infoBox.CandleClose = candle.Close.ToString("#.000");
                infoBox.CandleMax = candle.High.ToString("#.000");
                infoBox.CandleMin = candle.Low.ToString("#.000");
                infoBox.CandleVolume = candle.Volume.ToString("#.000");
                infoBox.CandleIndex = candleIndex.ToString();

                if (candleIndex > 16)
                {
                    infoBox.AtrValue = new SimpleAtr(ChartCalculator.Candles.TakeRange(candleIndex - 16, candleIndex).ToArray()).Calculate().ToString("#.000");
                }

                if (candleIndex > 16)
                {
                    infoBox.VolumeMedian = new CandlesStatisticsCalculator(ChartCalculator.Candles.TakeRange(candleIndex - 120, candleIndex).ToArray()).Calculate().Volume.Median.ToString("#.000");
                }

                //if (hasDeviation)
                //    infoBox.DeviationText = dp.ToString();
            }
            else
            {
                infoBox.Visibility = Visibility.Hidden;
            }
        }

        //TODO: не совсем корректно, например клик по свече это тоже клик по полигону.
        private Polygon _clickedTradeSignal;

        private void CanvasChartOnPreviewMouseUp(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 1)
            {
                var point = args.GetPosition((UIElement)sender);

                if (MouseUp != null)
                {
                    MouseUp(this, new StockChartMouseEventArgs(point));
                }
            }
        }



        private void CanvasChartOnPreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 2)
            {
                _clickedTradeSignal = null;

                Point pt = args.GetPosition((UIElement)sender);

                VisualTreeHelper.HitTest(canvasChart, null, MyHitTestResult, new PointHitTestParameters(pt));

                if (_clickedTradeSignal != null)
                {
                    var orderParams = (OrderParams)(_clickedTradeSignal).DataContext;

                    double minY = ViewPortDataHolder.ViewPortMinValue;
                    double maxY = ViewPortDataHolder.ViewPortMaxValue;

                    //var add = dp.Direction == Direction.Buy ? dp.Atr * 0.5 : dp.Atr * 0.5 * -1;
                    //var y = CanvasCalculator.GetCoordinateByValue((dp.FmaPrice + dp.SmaPrice) / 2 + add, minY, maxY);

                    var y = ChartCalculator.GetCoordinateByValue((double)orderParams.Price, minY, maxY);
                    var x1 = ChartCalculator.GetCoordinateByDate(ViewPortDataHolder.ViewPortStartDate);
                    var x2 = ChartCalculator.GetCoordinateByDate(ViewPortDataHolder.ViewPortEndDate);

                    var brush = orderParams.Side == OrderSides.Buy ? Brushes.Green : Brushes.Red;
                    DrawLine(canvasChart, x1, y, x2, y, brush, 1);

                    //MessageBox.Show(dp.Index.ToString());
                }
            }
            else if (args.ClickCount == 1)
            {
                var point = args.GetPosition((UIElement)sender);

                if (MouseDown != null)
                {
                    MouseDown(this, new StockChartMouseEventArgs(point));
                }
            }
        }

        private HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            var hit = result.VisualHit as Polygon;

            if (hit != null)
            {
                if (((Polygon)(result.VisualHit)).DataContext is OrderParams)
                {
                    _clickedTradeSignal = hit;
                    return HitTestResultBehavior.Stop;
                }
            }

            return HitTestResultBehavior.Continue;
        }

        StockChartRange _chartRange = new StockChartRange();

        private void CanvasChart_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (ChartCalculator == null) return;

            Point pt = args.GetPosition((UIElement)sender);
            var candle = ChartCalculator.GetCandleByCoordinate(pt.X);
            if (candle != null && _chartRange.Start != null && _chartRange.Start.Date != candle.Date)
            {
                _chartRange.End = candle;

                if (RangeSelected != null)
                {
                    RangeSelected(sender, new StockChartRangeSelectedEventArgs(_chartRange));
                }
            }

            _chartRange.Start = null;
        }

        private void CanvasChart_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (ChartCalculator != null)
            {
                Point pt = args.GetPosition((UIElement) sender);
                var candle = ChartCalculator.GetCandleByCoordinate(pt.X);
                if (candle != null)
                {
                    _chartRange.Start = candle;
                }
            }
        }

    }
}
