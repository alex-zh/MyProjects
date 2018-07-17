using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Classes;
using Visualizer.VisualControls.Classes;
using Visualizer.VisualControls.Controls.Classes;
using Visualizer.VisualControls.Controls.Classess;

namespace Visualizer.VisualControls.Controls
{
    public partial class TimeGraphChart
    {
        private string _lableX;
        private string _lableY;

        private TimeGraphChartCalculator _chartCalculator;
        private ChartCanvasCalculator _chartCanvasCalculator;
        private readonly List<Shape> _drawnPoints = new List<Shape>();
        private List<TimeGraphLine> _lines;
        private PlainChartPoint _currentPoint;
        private bool _shouldRedrawPointInfo = false;
        public new event EventHandler SizeChanged;

        public TimeGraphChart()
        {
            InitializeComponent();

            UseZeroY = true;
            WidthOffset = 60;
        }

        public bool UseZeroY { get; set; }
        public double WidthOffset { get; set; }

        public ChartTimeRanges ChartTimeRange { get; set; }

        private TimeGraphChartCalculator ChartCalculator
        {
            get
            {
                return _chartCalculator ?? (_chartCalculator = new TimeGraphChartCalculator(_lines, ChartTimeRange));
            }
            set
            {
                _chartCalculator = value;
            }
        }

        public ChartCanvasCalculator CanvasCalculator
        {
            get
            {
                var settings = new ChartCanvasCalculatorSettings()
                {
                    CanvasHeight = mainCanvas.ActualHeight,
                    CanvasWidth = mainCanvas.ActualWidth,
                    MinX = ChartCalculator.MinX,
                    MinY = ChartCalculator.MinY,
                    MaxX = ChartCalculator.MaxX,
                    MaxY = ChartCalculator.MaxY,
                    WidthOffset = WidthOffset
                };

                return _chartCanvasCalculator ?? (_chartCanvasCalculator = new ChartCanvasCalculator(settings));
            }
            set
            {
                _chartCanvasCalculator = value;
            }
        }

        public string Title
        {
            get { return (string)lblTitle.Content; }
            set { lblTitle.Content = value; }
        }

        public void Init(List<TimeGraphLine> lines)
        {
            Reset();

            _lines = lines;
        }


        public void InitLables(string labelX, string lableY)
        {
            _lableX = labelX.ToUpperFirstLetter();
            _lableY = lableY.ToUpperFirstLetter();
        }

        public void Draw()
        {
            if (_lines == null) { return; }

            DrawAxises();
            DrawLegends();
            DrawLines();
        }

        private void DrawLegends()
        {
            legendsPanel.Children.Clear();

            foreach (var graphLine in _lines)
            {
                legendsPanel.Children.Add(new Label
                {
                    Content = graphLine.Legend,
                    Foreground = new SolidColorBrush(graphLine.Color)
                });
            }
        }

        private void DrawAxises()
        {
            AddLable(mainCanvas, _lableX, new Point
            {
                X = CanvasCalculator.GetX((ChartCalculator.MinX + ChartCalculator.MaxX) / 2),
                Y = CanvasCalculator.GetY(ChartCalculator.MinY)
            });

            AddLable(mainCanvas, _lableY, new Point
            {
                X = CanvasCalculator.GetX(ChartCalculator.MinX) - 40,
                Y = CanvasCalculator.GetY((ChartCalculator.MinY + ChartCalculator.MaxY) / 2)
            }, true);

            DrawHorizontalAxis();

            DrawVerticalAxis();
        }

        private void DrawVerticalAxis()
        {
            var minX = ChartCalculator.MinX;
            var maxX = ChartCalculator.MaxX;
            var minY = ChartCalculator.MinY;
            var maxY = ChartCalculator.MaxY;

            AddLine(ChartCalculator.MinX, minY, minX, maxY, 0.5, Colors.Black);

            var ticks = new AxisTicksCalculator(minY, maxY).GetTicks(100).ToList();
            ticks = SubsampleTicksToFitCanvas(ticks, CanvasCalculator.CanvasHeight, tick => 1.25 * MeasureString(tick.ToString("N2")).Height);

            for (int i = 1; i < ticks.Count(); i++)
            {
                var tick = ticks[i];
                AddLine(CanvasCalculator.GetX(minX), CanvasCalculator.GetY(tick), CanvasCalculator.GetX(maxX), CanvasCalculator.GetY(tick), 0.5, Colors.Black, new double[] { 1, 4 }, true);

                var lableText = tick.ToString("N2");

                AddLable(mainCanvas, lableText, new Point()
                {
                    X = CanvasCalculator.GetX(minX) - MeasureString(lableText).Width - 5,
                    Y = CanvasCalculator.GetY(tick) - MeasureString(lableText).Height / 2
                });
            }
        }

        private void DrawHorizontalAxis()
        {
            var minX = (int)ChartCalculator.MinX;
            var maxX = (int)ChartCalculator.MaxX;
            var minY = ChartCalculator.MinY;
            var maxY = ChartCalculator.MaxY;
            var zeroY = 0;
            var axisYValue = UseZeroY ? zeroY : minY;

            AddLine(minX, axisYValue, maxX, axisYValue, 0.5, Colors.Black);

            var ticks = Enumerable.Range(minX, maxX - minX).ToList().ConvertAll(x => (double)x);
            ticks = SubsampleTicksToFitCanvas(ticks, CanvasCalculator.CanvasWidth, tick => 1.25 * MeasureString(GetTimeString((int)tick)).Width);

            for (int i = 1; i < ticks.Count(); i++)
            {
                var tick = ticks[i];
                AddLine(CanvasCalculator.GetX(tick), CanvasCalculator.GetY(minY), CanvasCalculator.GetX(tick), CanvasCalculator.GetY(maxY), 0.5, Colors.Black, new double[] { 1, 4 }, true);

                var lableText = GetTimeString((int) tick);

                AddLable(mainCanvas, lableText, new Point()
                {
                    X = CanvasCalculator.GetX(tick) - MeasureString(lableText).Width / 2,
                    Y = CanvasCalculator.GetY(axisYValue) + 10
                });
            }
        }

        private string GetTimeString(int tick)
        {
            if(ChartTimeRange == ChartTimeRanges.Day)
                return ChartCalculator.GetDateTimeFor(tick).ToString("dd/MM");

            if(ChartTimeRange == ChartTimeRanges.Minute)
                return ChartCalculator.GetDateTimeFor(tick).ToString("HH/mm");

            return "NA";
        }

        private List<double> SubsampleTicksToFitCanvas(List<double> ticks, double canvasSize, Func<double, double> measureFunction)
        {
            var subsampledTicks = new List<double>();
            subsampledTicks.AddRange(ticks);

            for (int i = 1; i < 20; i++)
            {
                subsampledTicks = subsampledTicks.Where((x, index) => index % i == 0).ToList();

                var neededWidth = subsampledTicks.Select(measureFunction).Sum();

                if (neededWidth < canvasSize)
                {
                    return subsampledTicks;
                }
            }

            return subsampledTicks;
        }

        private void AddLable(Canvas canvas, string text, Point pt, bool transpose = false)
        {
            var tb = GetLableTextBlock(text);

            if (transpose)
            {
                tb.RenderTransform = new RotateTransform(-90);
            }

            tb.Background = Brushes.Transparent;

            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            canvas.Children.Add(tb);

            Canvas.SetLeft(tb, pt.X);
            Canvas.SetTop(tb, pt.Y);
            Canvas.SetZIndex(tb, 1000);
        }

        private static TextBlock GetLableTextBlock(string text)
        {
            var tb = new TextBlock
            {
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Text = text,
                Background = Brushes.WhiteSmoke
            };
            return tb;
        }

        private void DrawLines()
        {
            foreach (var line in _lines)
            {
                DrawGraph(line);
            }
        }

        private void DrawGraph(TimeGraphLine graph)
        {
            var points = graph.Points.OrderBy(x => x.DateTime).ToList();

            for (int i = 1; i < points.Count; i++)
            {
                var previousPoint = points[i - 1];
                var point = points[i];

                var previousIndex = ChartCalculator.GetIndex(previousPoint);
                var index = ChartCalculator.GetIndex(point);

                var x1 = CanvasCalculator.GetX(previousIndex);
                var x2 = CanvasCalculator.GetX(index);

                var y1 = CanvasCalculator.GetY(previousPoint.Value);
                var y2 = CanvasCalculator.GetY(point.Value);

                AddLine(x1, y1, x2, y2, 1, graph.Color, null, true);

                AddPoint(x2, y2, graph.Color, point);
            }
        }

        private void AddPoint(double x, double y, Color color, PlainChartPoint point)
        {
            const int radius = 6;

            var ellipse = new Ellipse { Fill = new SolidColorBrush(color), Height = radius, Width = radius };

            Canvas.SetLeft(ellipse, x - radius / 2);
            Canvas.SetTop(ellipse, y - radius / 2);

            ellipse.DataContext = point;
            mainCanvas.Children.Add(ellipse);
        }

        private void Reset()
        {
            CanvasCalculator = null;
            ChartCalculator = null;

            _drawnPoints.Clear();
            mainCanvas.Children.Clear();
        }

        public Line AddLine(double a0, double a1, double thickness, Color color, double[] dashArray = null, bool areCanvasAjusted = false)
        {
            var x1 = ChartCalculator.MinX;
            var y1 = a0 + a1 * x1;

            var x2 = ChartCalculator.MaxX;
            var y2 = a0 + a1 * x2;

            return AddLine(x1, y1, x2, y2, thickness, color, dashArray, areCanvasAjusted);
        }

        public Line AddHorizontalLine(double y, double thickness, Color color, double[] dashArray = null, bool areCanvasAjusted = false)
        {
            return AddLine(ChartCalculator.MinX, y, ChartCalculator.MaxX, y, thickness, color, dashArray, areCanvasAjusted);
        }

        public Line AddVerticalLine(double x, double thickness, Color color, double[] dashArray = null, bool areCanvasAjusted = false)
        {
            return AddLine(x, ChartCalculator.MinY, x, ChartCalculator.MaxY, thickness, color, dashArray, areCanvasAjusted);
        }

        public Line AddLine(double x1, double y1, double x2, double y2, double thickness, Color color, double[] dashArray = null, bool areCanvasAjusted = false)
        {
            var canvasX1 = areCanvasAjusted ? x1 : CanvasCalculator.GetX(x1);
            var canvasX2 = areCanvasAjusted ? x2 : CanvasCalculator.GetX(x2);
            var canvasY1 = areCanvasAjusted ? y1 : CanvasCalculator.GetY(y1);
            var canvasY2 = areCanvasAjusted ? y2 : CanvasCalculator.GetY(y2);

            var line = new Line
            {
                X1 = canvasX1,
                X2 = canvasX2,
                Y1 = canvasY1,
                Y2 = canvasY2,
                StrokeThickness = thickness,
                Stroke = new SolidColorBrush(color)
            };

            if (dashArray != null)
            {
                line.StrokeDashArray = new DoubleCollection(dashArray);
            }

            mainCanvas.Children.Add(line);

            return line;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Reset();
            Draw();
            OnSizeChanged();
        }

        private Size MeasureString(string text)
        {
            var tb = GetLableTextBlock(text);

            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        protected void OnSizeChanged()
        {
            if (SizeChanged != null)
            {
                SizeChanged(this, EventArgs.Empty);
            }
        }

        public void ChartMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition((UIElement)sender);

            VisualTreeHelper.HitTest(mainCanvas, null, DataPointHitTestResult, new PointHitTestParameters(point));

            if (_currentPoint != null && _shouldRedrawPointInfo)
            {
                overlayCanvas.Children.Clear();
                ShowPointInfo(point.X, point.Y);                
            }
        }

        private void ShowPointInfo(double x, double y)
        {            
            var text = _currentPoint.ToString(ChartTimeRange);
            AddLable(overlayCanvas, text, new Point()
            {
                X = x + 5,
                Y = y - MeasureString(text).Height - 5
            });

        }

        private HitTestResultBehavior DataPointHitTestResult(HitTestResult result)
        {
            var hit = result.VisualHit as Ellipse;
            
            if (hit != null)
            {
                var dataContext = ((Ellipse)(result.VisualHit)).DataContext as PlainChartPoint;

                if (dataContext != null)
                {
                    if (_currentPoint != dataContext)
                    {
                        _shouldRedrawPointInfo = true;                        
                    }

                    _currentPoint = dataContext;
                    return HitTestResultBehavior.Stop;
                }                
            }

            _currentPoint = null;
            overlayCanvas.Children.Clear();

            return HitTestResultBehavior.Continue;
        }
    }
}
