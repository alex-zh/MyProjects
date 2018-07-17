using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Classes;
using Visualizer.VisualControls.Classes;
using Visualizer.VisualControls.Controls.Classes;
using Visualizer.VisualControls.Controls.Classess;

namespace Visualizer.VisualControls.Controls
{
    public partial class PlainChart
    {                
        private string _lableX;
        private string _lableY;        
        private List<PlainChartPoint> _points;

        private PlainChartCalculator _plainChartCalculator;
        private ChartCanvasCalculator _chartCanvasCalculator;
        private readonly List<Shape> _drawnPoints = new List<Shape>();
        public new event EventHandler SizeChanged;        

        public PlainChart()
        {
            InitializeComponent();

            UseZeroY = true;
        }

        public bool UseZeroY { get; set; }

        private PlainChartCalculator ChartCalculator
        {
            get
            {
                return _plainChartCalculator ?? (_plainChartCalculator = new PlainChartCalculator(_points));
            }
            set
            {
                _plainChartCalculator = value;
            }
        }

        public ChartCanvasCalculator CanvasCalculator
        {
            get {
                return _chartCanvasCalculator ?? (_chartCanvasCalculator = new ChartCanvasCalculator(mainCanvas.ActualHeight, 
                                                                                                                                 mainCanvas.ActualWidth,
                                                                                                                                 ChartCalculator.MinX,
                                                                                                                                 ChartCalculator.MinY,
                                                                                                                                 ChartCalculator.MaxX,
                                                                                                                                 ChartCalculator.MaxY));
            }
            set
            {
                _chartCanvasCalculator = value;
            }
        }

        public void Init(List<PlainChartPoint> points)
        {            
            Reset();
            
            _points = points;
        }

      
        public void InitLables(string labelX, string lableY)
        {
            _lableX = labelX.ToUpperFirstLetter();
            _lableY = lableY.ToUpperFirstLetter();
        }

        public void Draw()
        {
            if (_points == null) { return; }
            
            DrawAxises();            
            DrawPoints();            
        }

        private void DrawAxises()
        {            
            AddLable(_lableX, new Point
            {
                X = CanvasCalculator.GetX((ChartCalculator.MinX + ChartCalculator.MaxX) / 2),
                Y = CanvasCalculator.GetY(ChartCalculator.MinY)
            });

            AddLable(_lableY, new Point
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

            var ticks = new AxisTicksCalculator(minY, maxY).GetTicks().ToList();

            for (int i = 0; i < ticks.Count(); i++)
            {
                var tick = ticks[i];
                AddLine(CanvasCalculator.GetX(minX), CanvasCalculator.GetY(tick), CanvasCalculator.GetX(maxX), CanvasCalculator.GetY(tick), 0.5, Colors.Black, new double[] { 1, 4 }, true);

                var lableText = tick.ToString("N2");

                AddLable(lableText, new Point()
                {
                    X = CanvasCalculator.GetX(minX) - MeasureString(lableText).Width - 5,
                    Y = CanvasCalculator.GetY(tick) - MeasureString(lableText).Height / 2
                });
            }
        }

        private void DrawHorizontalAxis()
        {
            var minX = ChartCalculator.MinX;
            var maxX = ChartCalculator.MaxX;
            var minY = ChartCalculator.MinY;
            var maxY = ChartCalculator.MaxY;
            var zeroY = 0;
            var axisYValue = UseZeroY ? zeroY : minY;

            AddLine(minX, axisYValue, maxX, axisYValue, 0.5, Colors.Black);
            
            var ticks = new AxisTicksCalculator(minX, maxX).GetTicks().ToList();
            //var lables = PrepareLables(_points, ticks);
            
            //for (int i = 0; i < lables.Count; i++)
            //{
            //    var tickIndex = lables[i].Key;
            //    var tickText = lables[i].Value;
            //    AddLine(CanvasCalculator.GetX(tickIndex), CanvasCalculator.GetY(minY), CanvasCalculator.GetX(tickIndex), CanvasCalculator.GetY(maxY), 0.5, Colors.Black, new double[] { 1, 4 }, true);

            //    AddLable(tickText, new Point()
            //    {
            //        X = CanvasCalculator.GetX(tickIndex) - MeasureString(tickText).Width / 2,
            //        Y = CanvasCalculator.GetY(axisYValue) + 10
            //    });
            //}
        }


        private List<double> SubsampleTicksToFitCanvas(List<double> ticks, double canvasWidth, Func<double, double> measureFunction)
        {            
            for (int i = 1; i < 5; i++)
            {
                var subsampledTicks = ticks.Where((x, index) => index % i == 0).ToList();

                var neededWidth = subsampledTicks.Select(measureFunction).Sum();

                if (neededWidth < canvasWidth)
                {
                    return subsampledTicks;
                }    
            }
            
            return ticks;
        }

        private void AddLable(string text, Point pt, bool transpose = false)
        {
           var tb = GetLableTextBlock(text);

            if (transpose)
            {
                tb.RenderTransform = new RotateTransform(-90);
            }

            tb.Background = Brushes.Transparent;

            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            mainCanvas.Children.Add(tb);
            
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

        private void DrawPoints()
        {            
            //foreach (var p in _points)
            //{
            //    var ellipse = DrawPoint(p);

            //    _drawnPoints.Add(ellipse);
            //}
        }

        private Ellipse DrawPoint(BivariateChartPoint p)
        {
            var brush = p.Brush ?? Brushes.Red;
            var radius = p.Radius != 0 ? p.Radius : 6;

            var ellipse = new Ellipse { Fill = brush, Height = radius, Width = radius };

            Canvas.SetLeft(ellipse, CanvasCalculator.GetX(p.FirstValue) - 3);
            Canvas.SetTop(ellipse, CanvasCalculator.GetY(p.SecondValue) - 3);

            ellipse.DataContext = p;
            mainCanvas.Children.Add(ellipse);
            return ellipse;
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
            return AddLine(ChartCalculator.MinX, y, ChartCalculator.MaxX, y, thickness, color, dashArray , areCanvasAjusted);
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
    }
}
