using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Classes;
using Common.Classes.StrategyTester;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls
{
    public partial class DistributionChart
    {
        private CanvasCalculator _chartCanvasCalculator;
        private const int BarOffset = 1;
        private const int BarHalfWidth = 3;
        private List<DistributionBin> _bins;
        private int _minXValue;
        private int _maxXValue;
        private int _minYValue;
        private int _maxYValue;
        private DistributionBin _clickedBin;
        private DistributionParameters _parameters;
        private readonly Dictionary<DistributionCharacteristics, Line> _parametersLines = new Dictionary<DistributionCharacteristics, Line>();

        private Line _meanLine;
        private Line _medianLine;
        private Line _sigmaLine;
        private Line _medianSigmaLine;

        public string Title
        {
            get { return lblTitle.Text.ToString(); }
            set { lblTitle.Text = value; }
        }

        public DistributionChart()
        {
            InitializeComponent();
            ChartCanvas.PreviewMouseDown += CanvasChartOnPreviewMouseDown;
            this.IsVisibleChanged += DistributionChart_IsVisibleChanged;
        }
        
        void DistributionChart_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Draw();
        }

        public CanvasCalculator CanvasCalculator
        {
            get
            {
                if (_chartCanvasCalculator == null)
                {
                    _chartCanvasCalculator = new CanvasCalculator(ChartCanvas.ActualHeight, ChartCanvas.ActualWidth, BarHalfWidth * 2, BarOffset,
                                                                        _minXValue, _maxXValue, _minYValue, _maxYValue);
                }

                return _chartCanvasCalculator;
            }
        }

        public void Init(IEnumerable<DistributionBin> bins, DistributionParameters parameters, bool showNegative = false)
        {
            _bins = bins.ToList();
            _parameters = parameters;                        

            _minXValue = 0;
            _maxXValue = _bins.Count();

            if (showNegative)
                _minYValue = _bins.Any() ? _bins.Min(x => x.Count) : 0;
            else
                _minYValue = 0;

            _maxYValue = _bins.Any()? _bins.Max(x => x.Count):0;

            _chartCanvasCalculator = null;

            Draw();
        }        

        public void Draw()
        {
            if (IsVisible == false) return;

            if (_bins == null || _bins.Any() == false) return;            

            ChartCanvas.Children.Clear();

            //ChartCanvas.Width = Math.Max(gridCanvas.ActualWidth, CanvasCalculator.GetCanvasWidth(_bins.Count()));
            ChartCanvas.Width = CanvasCalculator.GetCanvasWidth(_bins.Count());
            gridCanvas.Width = Math.Max(gridCanvas.Width, ChartCanvas.Width);

            DrawVerticalAxis();
            DrawHorizontalAxis();

            for (int i = 0; i < _bins.Count; i++)
            {
                DrawBin(_bins[i], i);
            }
        }

        public void DrawParameter(double parameter, Color color, double[] dashArray = null)
        {
            AddLine(parameter, _minYValue, parameter, _maxYValue, 1, color);
        }

        private void DrawVerticalAxis()
        {
            //AddLine(_minXValue, _minYValue, _minXValue, _maxYValue, 0.5, Colors.Black);

            var ticks = new AxisTicksCalculator(_minYValue,_maxYValue).GetTicks().ToList();
            if (ticks.Count < 3) return;

            var addjustedTicks = new List<double> { ticks.First(), ticks[ticks.Count / 2], ticks.Last() };

            for (int i = 0; i < addjustedTicks.Count(); i++)
            {
                var tick = addjustedTicks[i];
                AddLine(CanvasCalculator.GetX(_minXValue), CanvasCalculator.GetY(tick), CanvasCalculator.GetX(_maxXValue), CanvasCalculator.GetY(tick), 0.5, Colors.Black, new double[] { 1, 4 }, true);

                var lableText = tick.ToString("N0");

                AddLable(lableText, new Point()
                {
                    X = CanvasCalculator.GetX(_minXValue) - MeasureString(lableText).Width - 10,
                    Y = CanvasCalculator.GetY(tick) - MeasureString(lableText).Height / 2
                });
            }
        }

        private void DrawHorizontalAxis()
        {
            //AddLine(_minXValue, _minYValue, _maxXValue, _minYValue, 0.5,  Colors.Gray);

             for (int i = 5; i < _maxXValue; i += 5)
            {
                
                //AddLine(CanvasCalculator.GetX(_minXValue), CanvasCalculator.GetY(tick), CanvasCalculator.GetX(_maxXValue), CanvasCalculator.GetY(tick), 0.5, Colors.Black, new double[] { 1, 4 }, true);

                var lableText = ((_bins[i].MinValue+_bins[i].MinValue)/2).ToString("N0");

                AddLable(lableText, new Point()
                {
                    X = CanvasCalculator.GetX(i) - MeasureString(lableText).Width / 2,
                    Y = CanvasCalculator.GetY(_minYValue) + MeasureString(lableText).Height
                });
            }          
        }

        private void DrawBin(DistributionBin bin, int index)
        {
            var binValue = bin.Count;
            var x = _chartCanvasCalculator.GetX(index);

            var openValue = _chartCanvasCalculator.GetY(0);
            var closeValue = _chartCanvasCalculator.GetY(binValue);

            var ptOpen1 = new Point(x, openValue);
            var ptOpen2 = new Point(x + 2 * BarHalfWidth, openValue);

            var ptClose1 = new Point(x + 2 * BarHalfWidth, closeValue);
            var ptClose2 = new Point(x, closeValue);

            var brush = Brushes.Red;
            var plg = new Polygon { Stroke = brush, StrokeThickness = 2 };

            Brush fillColor = Brushes.White;

            plg.Fill = fillColor;

            plg.Points.Add(ptOpen1);
            plg.Points.Add(ptOpen2);
            plg.Points.Add(ptClose1);
            plg.Points.Add(ptClose2);

            plg.DataContext = bin;

            ChartCanvas.Children.Add(plg);
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

            ChartCanvas.Children.Add(tb);

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

            ChartCanvas.Children.Add(line);

            return line;
        }

      private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _chartCanvasCalculator = null;

            if (_bins != null && _bins.Any())
            {
                Draw();
            }
        }

        private void CanvasChartOnPreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 2)
            {
                _clickedBin = null;

                Point pt = args.GetPosition((UIElement)sender);

                VisualTreeHelper.HitTest(ChartCanvas, null, BinClickedTestResult, new PointHitTestParameters(pt));

                if (_clickedBin != null)
                {
                    // получаем выбранный столбец
                }
            }
        }

        private HitTestResultBehavior BinClickedTestResult(HitTestResult result)
        {
            var hit = result.VisualHit as Polygon;

            if (hit != null)
            {
                if (((Polygon)(result.VisualHit)).DataContext is Deal)
                {
                    _clickedBin = (DistributionBin)hit.DataContext;
                    return HitTestResultBehavior.Stop;
                }
            }

            return HitTestResultBehavior.Continue;
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

        private void cbShowMean_Checked(object sender, RoutedEventArgs e)
        {            
            if (_parametersLines.ContainsKey(DistributionCharacteristics.Mean) &&
                ChartCanvas.Children.Contains(_parametersLines[DistributionCharacteristics.Mean]))
            {
                ChartCanvas.Children.Remove(_parametersLines[DistributionCharacteristics.Mean]);
            }

            if (cbShowMean.IsChecked != null && cbShowMean.IsChecked.Value)
            {
                var binIndex = GetBinIndex(_parameters.Mean);
                _parametersLines[DistributionCharacteristics.Mean] = AddLine(binIndex, _minYValue, binIndex, _maxYValue, 1, Colors.Blue);    
            }            
        }

        private double GetBinIndex(double value)
        {
            var binsWtihGreaterValue = _bins.SkipWhile(x => x.MaxValue < value).ToList();

            if (binsWtihGreaterValue.Any())
            {
                return _bins.IndexOf(binsWtihGreaterValue.First());
            }

            return _bins.Count - 1;
        }

        private void cbShowSigma_Checked(object sender, RoutedEventArgs e)
        {
            var sibmaCount = int.Parse(tbSigmas.Text);

            if (_parametersLines.ContainsKey(DistributionCharacteristics.Sigma) &&
                ChartCanvas.Children.Contains(_parametersLines[DistributionCharacteristics.Sigma]))
            {
                ChartCanvas.Children.Remove(_parametersLines[DistributionCharacteristics.Sigma]);
            }

            if (cbShowSigma.IsChecked != null && cbShowSigma.IsChecked.Value)
            {
                var binIndex = GetBinIndex( _parameters.Mean + _parameters.Sigma*sibmaCount);
                _parametersLines[DistributionCharacteristics.Sigma] = AddLine(binIndex, _minYValue, binIndex, _maxYValue, 1, Colors.Blue, new[] { 1.0, 3 });
            }            
        }

        private void cbShowMeadian_Checked(object sender, RoutedEventArgs e)
        {
            if (_parametersLines.ContainsKey(DistributionCharacteristics.Median) &&
                ChartCanvas.Children.Contains(_parametersLines[DistributionCharacteristics.Median]))
            {
                ChartCanvas.Children.Remove(_parametersLines[DistributionCharacteristics.Median]);
            }

            if (cbShowMedian.IsChecked != null && cbShowMedian.IsChecked.Value)
            {
                var binIndex = GetBinIndex(_parameters.Median);
                _parametersLines[DistributionCharacteristics.Median] = AddLine(binIndex, _minYValue, binIndex, _maxYValue, 1, Colors.BlueViolet);
            }                         
        }

        private void cbShowMedianSigma_Checked(object sender, RoutedEventArgs e)
        {
            var medianSigmasCount = int.Parse(tbMedianSigmas.Text);

            if (_parametersLines.ContainsKey(DistributionCharacteristics.SigmaMedian) &&
                ChartCanvas.Children.Contains(_parametersLines[DistributionCharacteristics.SigmaMedian]))
            {
                ChartCanvas.Children.Remove(_parametersLines[DistributionCharacteristics.SigmaMedian]);
            }

            if (cbShowMedianSigma.IsChecked != null && cbShowMedianSigma.IsChecked.Value)
            {
                var binIndex = GetBinIndex(_parameters.Median + _parameters.MedianSigma * medianSigmasCount);
                _parametersLines[DistributionCharacteristics.SigmaMedian] = AddLine(binIndex, _minYValue, binIndex, _maxYValue, 1, Colors.BlueViolet, new[] { 1.0, 3 });
            }                             
        }
    }
    
}
