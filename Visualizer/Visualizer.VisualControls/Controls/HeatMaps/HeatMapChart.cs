using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Classes;
using Visualizer.VisualControls.Classes;
using Visualizer.VisualControls.Controls.Classes;

namespace Visualizer.VisualControls.Controls.HeatMaps
{
    public abstract class HeatMapChart<TX, TY> : UserControl
        where TX : IComparable<TX>
        where TY : IComparable<TY>
    {
        private string _lableX;
        private string _lableY;

        private HeatMap<TX, TY> _heatMap;
        private HeatMapChartCalculator<TX, TY> _chartCalculator;
        private ChartCanvasCalculator _chartCanvasCalculator;

        public new event EventHandler SizeChanged;

        public abstract Canvas Canvas { get; }

        public abstract string FormatTitle(TX x);
        public abstract string FormatTitle(TY y);

        public bool ShowLables { get; set; }

        private HeatMapChartCalculator<TX, TY> ChartCalculator
        {
            get
            {
                return _chartCalculator ?? (_chartCalculator = new HeatMapChartCalculator<TX, TY>(_heatMap));
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
                    CanvasHeight = Canvas.ActualHeight,
                    CanvasWidth = Canvas.ActualWidth,
                    MinX = ChartCalculator.MinX,
                    MinY = ChartCalculator.MinY,
                    MaxX = ChartCalculator.MaxX + 1,
                    MaxY = ChartCalculator.MaxY + 1,
                    WidthOffset = 60
                };

                return _chartCanvasCalculator ?? (_chartCanvasCalculator = new ChartCanvasCalculator(settings));
            }
            set
            {
                _chartCanvasCalculator = value;
            }
        }

        //public string Title
        //{
        //    get { return (string)lblTitle.Content; }
        //    set { lblTitle.Content = value; }
        //}

        public double RectHeight => CanvasCalculator.CanvasHeight / (_chartCalculator.MaxY + 1);

        public double RectWidth
        {

            get
            {
                return CanvasCalculator.CanvasWidth / (_chartCalculator.MaxX + 1);

            }
        }


        public void Init(HeatMap<TX, TY> heatMap)
        {
            Reset();

            _heatMap = heatMap;
        }


        public void InitLables(string labelX, string lableY)
        {
            _lableX = labelX.ToUpperFirstLetter();
            _lableY = lableY.ToUpperFirstLetter();
        }

        public void Draw()
        {
            if (_heatMap.Any() == false) { return; }

            DrawAxises();
            DrawLegends();
            DrawMap();
        }

        private void DrawLegends()
        {
            // legendsPanel.Children.Clear();

            //foreach (var graphLine in _lines)
            //{
            //    legendsPanel.Children.Add(new Label
            //    {
            //        Content = graphLine.Legend,
            //        Foreground = new SolidColorBrush(graphLine.Color)
            //    });
            //}
        }

        private void DrawAxises()
        {
            AddLable(Canvas, _lableX, new Point
            {
                X = CanvasCalculator.GetX((ChartCalculator.MinX + ChartCalculator.MaxX) / 2),
                Y = CanvasCalculator.GetY(ChartCalculator.MinY)
            });

            AddLable(Canvas, _lableY, new Point
            {
                X = CanvasCalculator.GetX(ChartCalculator.MinX) - 40,
                Y = CanvasCalculator.GetY((ChartCalculator.MinY + ChartCalculator.MaxY) / 2)
            }, true);

            DrawHorizontalAxis();

            DrawVerticalAxis();
        }

        private void DrawVerticalAxis()
        {
            var minX = (int)ChartCalculator.MinX;
            var maxX = (int)ChartCalculator.MaxX;
            var minY = ChartCalculator.MinY;
            var maxY = ChartCalculator.MaxY;

            AddLine(minX, minY, minX, maxY + 1, 0.5, Colors.Black);

            var ticks = new AxisTicksCalculator(minY, maxY).GetTicks(100, new TicksSettings { OnlyInteger = true }).ToList();
            ticks = SubsampleTicksToFitCanvas(ticks, CanvasCalculator.CanvasHeight, tick => 1.25 * MeasureString(GetYTitleString((int)tick)).Height);

            for (int i = 0; i < ticks.Count(); i++)
            {
                var tick = ticks[i];

                var lableText = GetYTitleString((int)tick);

                AddLable(Canvas, lableText, new Point()
                {
                    X = CanvasCalculator.GetX(minX) - MeasureString(lableText).Width - 5,
                    Y = CanvasCalculator.GetY(tick) - RectHeight / 2 - MeasureString(lableText).Height / 2
                });
            }
        }

        private void DrawHorizontalAxis()
        {
            var minX = (int)ChartCalculator.MinX;
            var maxX = (int)ChartCalculator.MaxX;
            var minY = ChartCalculator.MinY;
            var maxY = ChartCalculator.MaxY;

            AddLine(minX, minY, maxX + 1, minY, 0.5, Colors.Black);

            var ticks = new AxisTicksCalculator(minX, maxX).GetTicks(100, new TicksSettings { OnlyInteger = true }).ToList();
            ticks = SubsampleTicksToFitCanvas(ticks, CanvasCalculator.CanvasWidth, tick => 1.25 * MeasureString(GetXTitleString((int)tick)).Width);

            for (int i = 0; i < ticks.Count(); i++)
            {
                var tick = ticks[i];

                var lableText = GetXTitleString((int)tick);

                AddLable(Canvas, lableText, new Point()
                {
                    X = CanvasCalculator.GetX(tick) + RectWidth / 2 - MeasureString(lableText).Width / 2,
                    Y = CanvasCalculator.GetY(minY) + 10
                });
            }
        }

        private string GetXTitleString(int tick)
        {
            return FormatTitle(_heatMap.GetX(tick));
        }

        private string GetYTitleString(int tick)
        {
            return FormatTitle(_heatMap.GetY(tick));
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

        private void DrawMap()
        {
            double rectWidth = RectWidth;
            double rectHeight = RectHeight;

            for (int x = 0; x <= _heatMap.MaxX; x++)
            {
                for (int y = 0; y <= _heatMap.MaxY; y++)
                {
                    var x1 = CanvasCalculator.GetX(x);
                    var y1 = CanvasCalculator.GetY(y) - rectHeight;

                    var rect = new Rectangle
                    {
                        StrokeThickness = 0,
                        Width = rectWidth,
                        Height = rectHeight,
                        Fill =
                            new HeatMapColorProvider(_heatMap.MinValue, _heatMap.MaxValue).GetBrush(_heatMap.GetValue(
                                x, y))
                    };

                    if (ShowLables)
                    {
                        var label = _heatMap.GetValue(x, y).ToString("N2");
                        AddLable(Canvas, label,
                            new Point()
                            {
                                X = x1 + rectWidth / 2 - MeasureString(label).Width / 2,
                                Y = y1 + rectHeight / 2 - MeasureString(label).Height / 2
                            });
                    }

                    Canvas.SetLeft(rect, x1);
                    Canvas.SetTop(rect, y1);
                    Canvas.Children.Add(rect);
                }
            }
        }

        private void AddPoint(double x, double y, Color color, PlainChartPoint point)
        {
            const int radius = 6;

            var ellipse = new Ellipse { Fill = new SolidColorBrush(color), Height = radius, Width = radius };

            Canvas.SetLeft(ellipse, x - radius / 2);
            Canvas.SetTop(ellipse, y - radius / 2);

            ellipse.DataContext = point;
            Canvas.Children.Add(ellipse);
        }

        private void Reset()
        {
            CanvasCalculator = null;
            ChartCalculator = null;

            Canvas.Children.Clear();
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

            Canvas.Children.Add(line);

            return line;
        }

        public void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            if (_heatMap == null)
                return;

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

            VisualTreeHelper.HitTest(Canvas, null, DataPointHitTestResult, new PointHitTestParameters(point));

            //if (_currentPoint != null && _shouldRedrawPointInfo)
            //{
            //    overlayCanvas.Children.Clear();
            //    ShowPointInfo(point.X, point.Y);                
            //}
        }

        private void ShowPointInfo(double x, double y)
        {
            //var text = _currentPoint.ToString(ChartTimeRange);
            //AddLable(overlayCanvas, text, new Point()
            //{
            //    X = x + 5,
            //    Y = y - MeasureString(text).Height - 5
            //});

        }

        private HitTestResultBehavior DataPointHitTestResult(HitTestResult result)
        {
            var hit = result.VisualHit as Ellipse;

            //if (hit != null)
            //{
            //    var dataContext = ((Ellipse)(result.VisualHit)).DataContext as PlainChartPoint;

            //    if (dataContext != null)
            //    {
            //        if (_currentPoint != dataContext)
            //        {
            //            _shouldRedrawPointInfo = true;                        
            //        }

            //        _currentPoint = dataContext;
            //        return HitTestResultBehavior.Stop;
            //    }                
            //}

            //_currentPoint = null;
            //overlayCanvas.Children.Clear();

            return HitTestResultBehavior.Continue;
        }
    }

}