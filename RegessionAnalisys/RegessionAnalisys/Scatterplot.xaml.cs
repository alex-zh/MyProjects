using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Scatterplot;

namespace RegessionAnalisys
{
    public partial class Scatterplot
    {
        private TextBlock _pointInfoTextBlock;
        private CanvasCalculator _canvasCalculator;
        private string _lableX;
        private string _lableY;
        private Outliers _outliers;
        private ScatterPointCollection _points;
        private ScatterplotCalculator _scatterplotCalculator;
        private Line _zeroVerticalLine;
        private Line _zeroHorizontalLine;        

        public Scatterplot()
        {
            InitializeComponent();
        }

        public bool ShowZeroAxises { get; set; }

        public ScatterplotCalculator ScatterplotCalculator
        {
            get
            {
                return _scatterplotCalculator ?? (_scatterplotCalculator = new ScatterplotCalculator(_points));
            }
            set
            {
                _scatterplotCalculator = value;
            }
        }

        public CanvasCalculator CanvasCalculator
        {
            get {
                return _canvasCalculator ?? (
                    _canvasCalculator = new CanvasCalculator(mainCanvas.ActualHeight, 
                                                             mainCanvas.ActualWidth, 
                                                             ScatterplotCalculator.MinX, 
                                                             ScatterplotCalculator.MinY,
                                                             ScatterplotCalculator.MaxX,
                                                             ScatterplotCalculator.MaxY));
            }
            set
            {
                _canvasCalculator = value;
            }
        }        

        public void Init(ScatterPointCollection points, Outliers outliers, string labelX, string lableY)
        {            
            Reset();

            _lableX = labelX.ToUpperFirstLetter();
            _lableY = lableY.ToUpperFirstLetter();
            _points = points;
            _outliers = outliers;         
        }

        private void Draw()
        {
            if (_points == null) { return; }
            
            DrawAxises();
            DrawZeroAxises();
            DrawPoints();            
        }

        private void DrawAxises()
        {            
            double minX = ScatterplotCalculator.MinX;
            double minY = ScatterplotCalculator.MinY;
            double maxX = ScatterplotCalculator.MaxX;
            double maxY = ScatterplotCalculator.MaxY;

            AddLable(_lableX, new Point()
            {
                X = CanvasCalculator.GetX((minX + maxX) / 2),
                Y = CanvasCalculator.GetY(minY)
            });

            AddLable(_lableY, new Point()
            {
                X = CanvasCalculator.GetX(minX) - 20,
                Y = CanvasCalculator.GetY((minY + maxY) / 2)
            }, true);

            AddLine(minX, minY, minX, maxY, 1, Colors.Black);

            AddLine(CanvasCalculator.GetX(minX), CanvasCalculator.GetY(maxY), CanvasCalculator.GetX(minX) + 3, CanvasCalculator.GetY(maxY) + 5, 1, Colors.Black, null, true);
            AddLine(CanvasCalculator.GetX(minX), CanvasCalculator.GetY(maxY), CanvasCalculator.GetX(minX) - 3, CanvasCalculator.GetY(maxY) + 5, 1, Colors.Black, null, true);
          
            AddLine(minX, minY, maxX, minY, 1, Colors.Black);            
        }

        private void DrawZeroAxises()
        {
            if (ShowZeroAxises)
            {
                if (ScatterplotCalculator.MinX < 0 && ScatterplotCalculator.MaxX > 0)
                {
                    _zeroVerticalLine = AddLine(0, ScatterplotCalculator.MinY, 0, ScatterplotCalculator.MaxY, 0.5, Colors.Black, new Double[] { 2, 4 });                 
                }

                if (ScatterplotCalculator.MinY < 0 && ScatterplotCalculator.MaxY > 0)
                {
                    _zeroHorizontalLine = AddLine(ScatterplotCalculator.MinX, 0, ScatterplotCalculator.MaxX, 0, 0.5, Colors.Black, new Double[] { 2, 4 });                                     
                }
            }
            else
            {
                if (_zeroHorizontalLine != null)
                {
                    mainCanvas.Children.Remove(_zeroHorizontalLine);
                }
                if (_zeroVerticalLine != null)
                {
                    mainCanvas.Children.Remove(_zeroVerticalLine);
                }
            }
        }

        private void AddLable(string text, Point pt, bool transpose = false)
        {
           var tb = new TextBlock
            {
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Text = text,
                Background = Brushes.WhiteSmoke
            };

            if (transpose)
            {
                tb.RenderTransform = new RotateTransform(-90);
            }

            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            mainCanvas.Children.Add(tb);

            Canvas.SetLeft(tb, pt.X);
            Canvas.SetTop(tb, pt.Y);
        }

        public Line AddLine(double x1, double y1, double x2, double y2, double thickness, Color color, double[] dashArray = null, bool areCanvasAjusted = false)
        {
            var canvasX1 = areCanvasAjusted? x1: CanvasCalculator.GetX(x1);
            var canvasX2 = areCanvasAjusted? x2: CanvasCalculator.GetX(x2);
            var canvasY1 = areCanvasAjusted? y1: CanvasCalculator.GetY(y1);
            var canvasY2 = areCanvasAjusted? y2 : CanvasCalculator.GetY(y2);            

            var line = new Line()
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

        private void DrawPoints()
        {            
            foreach (var p in _points)
            {
                var color = new ColorPicker().Pick(p.Group);
                var brush = new SolidColorBrush(color);

                var ellipse = new Ellipse() { Fill = brush, Height = 10, Width = 10 };

                if (Math.Abs(p.SubGroup) != 0)
                {
                    ellipse.StrokeThickness = 2;
                    ellipse.Stroke = new SolidColorBrush(new ColorPicker().RevertColor(color));                    
                }

                if (_outliers != null && _outliers.Y.Contains(p.Y))
                {
                    ellipse.Height = 20;
                    ellipse.Width = 20;
                }

                if (_outliers != null && _outliers.Y.Contains(p.X))
                {
                    ellipse.Height = 20;
                    ellipse.Width = 20;
                }

                Canvas.SetLeft(ellipse, CanvasCalculator.GetX(p.X) - 5);
                Canvas.SetTop(ellipse, CanvasCalculator.GetY(p.Y) - 5);

                ellipse.DataContext = p;
                mainCanvas.Children.Add(ellipse);
            }
        }

        private void Reset()
        {
            CanvasCalculator = null;
            ScatterplotCalculator = null;
            mainCanvas.Children.Clear();         
        }

        private void mainCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition((Canvas)sender);
            HitTestResult result = VisualTreeHelper.HitTest(mainCanvas, pt);

            if (result != null)
            {
                var v = result.VisualHit as Shape;
                if (v != null && v.DataContext != null)
                {
                    var p = (ScatterplotPoint)v.DataContext;
                    var text = _lableX + ": " + p.X + _lableY + ": " + p.Y;

                    AddText(text, pt);
                }
            }
        }

        private void AddText(string text, Point pt)
        {
            _pointInfoTextBlock = new TextBlock
            {
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Text = text,
                Background = Brushes.WhiteSmoke
            };
            _pointInfoTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var xCoordinate = pt.X + 10;
            if (pt.X + _pointInfoTextBlock.DesiredSize.Width > mainCanvas.ActualWidth)
            {
                xCoordinate = pt.X - _pointInfoTextBlock.DesiredSize.Width - 10;
            }
            mainCanvas.Children.Add(_pointInfoTextBlock);

            Canvas.SetLeft(_pointInfoTextBlock, xCoordinate);
            Canvas.SetTop(_pointInfoTextBlock, pt.Y - 10);
        }

        private void mainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
         
        }

        private void mainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {            
            if (_pointInfoTextBlock != null)
            {
                mainCanvas.Children.Remove(_pointInfoTextBlock);
            }
            _pointInfoTextBlock = null;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Reset();
            Draw();
        }
    }
}
