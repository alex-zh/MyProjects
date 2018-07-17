using System;
using System.Collections.Generic;
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
using Common.Finders;
using Robot;
using Robot.Strategies;
using Visualizer.Core.Classes;

namespace Visualizer.Core
{
    public partial class StockChart : IStockChart
    {
        private const int CandleHalfWidth = 3;
        private const int CandleOffset = 3;
        private List<IChartMovingAverageLine> emaList = new List<IChartMovingAverageLine>();
        //private List<IChartLine> deviationLines = new List<IChartLine>();
        //private List<AbnormalDeviationPoint> deviatedPoints = new List<AbnormalDeviationPoint>();

        public StockChart()
        {
            InitializeComponent();
            //canvasChart.Background = Brushes.Aqua; 
            //canvasChart.PreviewMouseLeftButtonUp += canvasChart_MouseLeftButtonUp;
            canvasChart.PreviewMouseDown += CanvasChartOnPreviewMouseDown ;
            gridChart.SizeChanged += gridChart_SizeChanged;
            scrollBarHorizontal.ValueChanged += scrollBarHorizontal_ValueChanged;

            ViewPortDataHolder = new ViewPortDataHolder();
        }

        public ViewPortDataHolder ViewPortDataHolder
        {
            get;
            private set;
        }

        public ChartCalculator ChartCalculator
        {
            get;
            set;
        }

        public bool IsReady
        {
            get
            {
                return ChartCalculator != null &&
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

        private void SynchronizeScrollBars()
        {
            scrollBarHorizontal.Minimum = 0;
            scrollBarHorizontal.Maximum = scrollViewerCanvasChart.ScrollableWidth;
            scrollBarHorizontal.ViewportSize = scrollViewerCanvasChart.ViewportWidth;
        }

        private void SynchronizeScrollPosition(double newValue)
        {
            scrollViewerCanvasChart.ScrollToHorizontalOffset(newValue);
            scrollViewerDateTimeAxis.ScrollToHorizontalOffset(newValue);
            //scrollViewerIndicatorChart.ScrollToHorizontalOffset(newValue);
        }

        private void DrawVerticalGridLines()
        {
            var minY = ViewPortDataHolder.ViewPortMinValue;
            var maxY = ViewPortDataHolder.ViewPortMaxValue;

            var ticksCalc = new ValueAxisTicksCalculator(minY, maxY);

            foreach (double dy in ticksCalc.GetTicks())
            {
                var gridline = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.3,
                    StrokeDashArray = new DoubleCollection(new double[2] { 3, 9 }),
                    X1 = 0,
                    X2 = canvasChart.Width,
                    Y1 = ChartCalculator.GetCoordinateByValue(dy, minY, maxY)
                };

                gridline.Y2 = gridline.Y1;

                canvasChart.Children.Add(gridline);
            }
        }

        private List<int> tempList;
        private double upThesholdValue, downThesholdValue;

        private List<DivergencyPoint> _divergencyPoints;
        public void Init(IEnumerable<Candle> candles)
        {
            emaList.Clear();

            ChartCalculator = new ChartCalculator(candles,
                                                  CandleHalfWidth,
                                                  CandleOffset,
                                                  canvasChart.Width,
                                                  canvasChart.Height);

            // emaList.Add(new MovingAverage(candles, 11, Brushes.Red, 2));
            emaList.Add(new ChartMovingAverage(candles, 11, PriceTypes.Mean, Brushes.Red, 1));
            emaList.Add(new ChartMovingAverage(candles, 21, PriceTypes.Mean, Brushes.Maroon, 1));

            var divergencyFinder = new MovingsDivergencyFinder(candles, 11, 21);
            _divergencyPoints = divergencyFinder.Find().ToList();

            //List<DatePrice> upValues, downValues;
            //var d = new DeviationFromMeanFinder(candles);
            //tempList = d.GetStatistics(out downValues, out upValues).ToList();

            //deviationLines.Add(new ChartMovingAverage(upValues, 11, PriceTypes.Close, Brushes.Green, 1));
            //deviationLines.Add(new ChartMovingAverage(downValues, 11, Brushes.Red, 1));
            //var bins = d.GetBins();

            //var dumper = new Dumper<object>(@"C:\result\bins.txt");
            //foreach (var bin in bins)
            //{
            //    dumper.WriteLine(bin.ToString());
            //}
            //var deviationCorrector = new DeviationCorrector(candles, 
            //                                                new VolatilityCalculator(candles), 
            //                                                new DeviationFinder(candles)); 

            //deviationCorrector.Calculate();
            //new Dumper<List<VolatilityGroup>>(@"C:\result\dump.txt").Dump( deviationCorrector._volatilityGroups);

            //deviatedPoints = new DeviationFinder(candles).Find().ToList();


            PrepareAndDrawChart();
        }

        public void Draw()
        {
            this.Visibility = Visibility.Visible;

            canvasChart.Children.Clear();

            DrawVerticalGridLines();

            double minY = ViewPortDataHolder.ViewPortMinValue;
            double maxY = ViewPortDataHolder.ViewPortMaxValue;

            foreach (var candle in ChartCalculator.Candles)
            {
                if (candle.Date < ViewPortDataHolder.ViewPortStartDate || candle.Date > ViewPortDataHolder.ViewPortEndDate)
                {
                    continue;
                }

                DrawCandle(candle, minY, maxY);
                DrawEmas(candle, minY, maxY);
                //DrawTradeSignals(candle, minY, maxY);
                //DrawStrategyTradeSignals(candle, minY, maxY);
            }
            
        }


        private void DrawEmas(Candle candle, double minY, double maxY)
        {
            var index = ChartCalculator.IndexOf(candle);

            var x1 = ChartCalculator.GetCoordinateByIndex(index - 1);
            var x2 = ChartCalculator.GetCoordinateByIndex(index);

            foreach (var ema in emaList)
            {
                if (index <= ema.Period) continue;

                var previousValue = ema[index - 1].Value;
                var currentValue = ema[index].Value;

                var y1 = ChartCalculator.GetCoordinateByValue(previousValue, minY, maxY);
                var y2 = ChartCalculator.GetCoordinateByValue(currentValue, minY, maxY);

                DrawLine(canvasChart, x1, y1, x2, y2, ema.Color, ema.LineThickness);
            }

            //foreach (var ema in deviationLines)
            //{
            //    if (index <= ema.Period) continue;

            //    var previousValue = ema[index - 1].Price;
            //    var currentValue = ema[index].Price;

            //    var y1 = ChartCalculator.GetCoordinateByValue(previousValue, minY, maxY);
            //    var y2 = ChartCalculator.GetCoordinateByValue(currentValue, minY, maxY);

            //    DrawLine(canvasChart, x1, y1, x2, y2, ema.Color, ema.LineThickness);
            //}
        }

        private void DrawTradeSignals(Candle candle, double minY, double maxY)
        {
            var divergencyPoint = _divergencyPoints.FirstOrDefault(x => x.Date == candle.Date);

            if (divergencyPoint != null)
            {
                var candleValue = (divergencyPoint.Direction == Direction.Up) ? candle.High : candle.Low;

                var coordinateX = ChartCalculator.GetCoordinateByDate(divergencyPoint.Date);
                var coordinateY = ChartCalculator.GetCoordinateByValue(candleValue, minY, maxY);

                DrawTradeTriangle(coordinateX, coordinateY, divergencyPoint);
                
            }
        }
       
        private void DrawTradeTriangle(double coordinateX, double coordinateY, DivergencyPoint point)
        {
            var brushColor = point.Direction == Direction.Up ? Brushes.Green : Brushes.Red;

            var plg = new Polygon {Stroke = brushColor, StrokeThickness = 1, Fill = brushColor, DataContext = point};
            
            if (point.Direction == Direction.Up)
            {
                plg.Points.Add(new Point(coordinateX - 5, coordinateY - 3));
                plg.Points.Add(new Point(coordinateX, coordinateY - 13));
                plg.Points.Add(new Point(coordinateX + 5, coordinateY - 3));
                plg.Points.Add(new Point(coordinateX - 5, coordinateY - 3));
            }
            else
            {
                plg.Points.Add(new Point(coordinateX - 5, coordinateY + 3));
                plg.Points.Add(new Point(coordinateX, coordinateY + 13));
                plg.Points.Add(new Point(coordinateX + 5, coordinateY + 3));
                plg.Points.Add(new Point(coordinateX - 5, coordinateY + 3));
            }
            
            canvasChart.Children.Add(plg);
        }

        
        void plg_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        void plg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            
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

            var candleIndex = ChartCalculator.IndexOf(candle);


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
            var x = e.GetPosition(canvasChart).X;
            var y = e.GetPosition(canvasChart).Y;

            Task.Run(() =>
            {
                Thread.Sleep(50);

                canvasChart.Dispatcher.Invoke(() =>
                {
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

                    AbnormalDeviationPoint dp = null;
                    // var hasDeviation = deviatedPoints.Any(p => p.Date == candle.Date);

                    //if(hasDeviation)
                    //     dp = deviatedPoints.Single(p => p.Date == candle.Date);

                    cursorInfoBox.Visibility = Visibility.Visible;
                    cursorInfoBox.Value = ChartCalculator.GetValueByCoordinate(y, minY, maxY).ToString("####.000");

                    if (y >= Math.Min(closeValue, openValue) && y <= Math.Max(closeValue, openValue) &&
                        x >= coordinateX - CandleHalfWidth && x <= coordinateX + CandleHalfWidth)
                    {
                        infoBox.Visibility = Visibility.Visible;
                        infoBox.CandleDate = candle.Date.ToString();
                        infoBox.CandleOpen = candle.Open.ToString("#.000");
                        infoBox.CandleClose = candle.Close.ToString("#.000");
                        infoBox.CandleMax = candle.High.ToString("#.000");
                        infoBox.CandleMin = candle.Low.ToString("#.000");
                        infoBox.CandleIndex = ChartCalculator.Candles.ToList().IndexOf(candle).ToString();
                        //if (hasDeviation)
                        //    infoBox.DeviationText = dp.ToString();
                    }
                    else
                    {
                        infoBox.Visibility = Visibility.Hidden;
                    }
                });
            });
        }

        //TODO: не совсем корректно, например клик по свече это тоже клик по полигону.
        private Polygon _clickedTradeSignal;

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

                    //var add = dp.Direction == Direction.Up ? dp.Atr * 0.5 : dp.Atr * 0.5 * -1;
                    //var y = ChartCalculator.GetCoordinateByValue((dp.FmaPrice + dp.SmaPrice) / 2 + add, minY, maxY);

                    var y = ChartCalculator.GetCoordinateByValue((double)orderParams.Price, minY, maxY);
                    var x1 = ChartCalculator.GetCoordinateByDate(ViewPortDataHolder.ViewPortStartDate);
                    var x2 = ChartCalculator.GetCoordinateByDate(ViewPortDataHolder.ViewPortEndDate);

                    var brush = orderParams.Direction == Direction.Up ? Brushes.Green : Brushes.Red;
                    DrawLine(canvasChart, x1, y, x2, y, brush, 1);

                    //MessageBox.Show(dp.Index.ToString());
                }
            }
        }

        private HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            var hit = result.VisualHit as Polygon;

            if (hit != null)
            {
                if (((Polygon) (result.VisualHit)).DataContext is OrderParams)
                {
                    _clickedTradeSignal = hit;
                    return HitTestResultBehavior.Stop;        
                }
            }

            return HitTestResultBehavior.Continue;
        }
    }
}
