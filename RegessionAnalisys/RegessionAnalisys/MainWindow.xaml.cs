using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Regression;
using Common.Scatterplot;
using Microsoft.Win32;
using RegessionAnalisys.DataPreparers;
using Scatterplot.Core.Classes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace RegessionAnalisys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBlock tb;
        private string _lableX;
        private string _lableY;
        private Line _zeroVerticalLine, _zeroHorizontalLine;
        private CanvasCalculator _canvasCalculator;
        private ScatterPointCollection _points;
        private ScatterPointCollection _residuals;
        private ScatterplotCalculator _scatterplotCalculator;

        public MainWindow()
        {
            InitializeComponent();
            BindDataFiles();
        }

        public bool ShowZeroAxises
        {
            get
            {
                return cbDrawZeroAxises.IsChecked.HasValue && cbDrawZeroAxises.IsChecked.Value;

            }
        }

        private void BindDataFiles()
        {
            cbDataFiles.ItemsSource = new DataFilesProvider().GetList();
        }

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
            get
            {
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

        public void Init(string labelX, string lableY, ScatterPointCollection points)
        {
            ResetCanvas();

            _lableX = labelX;
            _lableY = lableY;
            _points = points;

            Draw();
        }

        private void DrawRegression()
        {
            //основная регрессия, по всем точкам, без учета категории
            DrawRegressionLine(_points.ToArray(), 5);

            for (var groupIndex = 0; groupIndex < _points.GroupsCount; groupIndex++)
            {
                DrawRegressionLine(_points.Groups[groupIndex]);
            }
        }

        private void DrawRegressionLine(int groupIndex)
        {
            var categoryPoints = _points.Where(x => Math.Abs(x.Group) == groupIndex).ToArray();

            DrawRegressionLine(categoryPoints, groupIndex);
        }

        private void DrawRegressionLine(ScatterplotPoint[] groupPoints, int groupIndex)
        {
            var regression = new LinearRegressionBuilder(groupPoints.Select(x => x.X).ToArray(), groupPoints.Select(x => x.Y).ToArray()).Build();

            var categoryCalculator = new ScatterplotCalculator(groupPoints);

            var x1 = CanvasCalculator.GetX(categoryCalculator.MinX);
            var y1 = CanvasCalculator.GetY(regression.a0 + regression.a1 * categoryCalculator.MinX);

            var x2 = CanvasCalculator.GetX(categoryCalculator.MaxX);
            var y2 = CanvasCalculator.GetY(regression.a0 + regression.a1 * categoryCalculator.MaxX);

            var vLine = new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(new ColorPicker().Pick(groupIndex))
            };

            mainCanvas.Children.Add(vLine);

            DisplayResult(groupIndex, regression);
        }

        private void DisplayResult(int category, LinearRegression regression)
        {
            var colorPicker = new ColorPicker();
            tbInfo.AppendText("\n");
            tbInfo.AppendText(String.Format("Group: '{0}' ({1})", category, colorPicker.GetName(colorPicker.Pick(category))));
            tbInfo.AppendText("\n");
            tbInfo.AppendText(String.Format("Regression: {0} + {1} * x ", regression.a0.ToString("F2"), regression.a1.ToString("F2")));
            tbInfo.AppendText("\n");
            tbInfo.AppendText(String.Format("R-squared: {0}", regression.RSquared.ToString("F2")));
            tbInfo.AppendText("\n");
            tbInfo.AppendText(String.Format("SEE: {0}", regression.StandardEstimateError.ToString("F2")));
            tbInfo.AppendText("\n");
            tbInfo.AppendText("--------------------");
        }

        private void Draw()
        {
            if (_points == null) return;

            //DrawLables(_points);
            DrawAxises();
            DrawZeroAxises(_points);
            DrawPoints(_points, true);
            DrawRegression();
        }

        private void DrawLables()
        {

        }

        private void DrawPoints(ScatterPointCollection points, bool drawOutliers)
        {
            IEnumerable<double> outliersY = new Double[0];
            IEnumerable<double> outliersX = new Double[0];

            if (drawOutliers)
            {
                var f = new ThreeSigmaOutliersFinder(points.Select(p => p.Y));
                outliersY = f.Find();
                f = new ThreeSigmaOutliersFinder(points.Select(p => p.X));
                outliersX = f.Find();
            }

            var calculator = new ScatterplotCalculator(points);

            foreach (var p in points)
            {
                var color = new ColorPicker().Pick(p.Group);
                var brush = new SolidColorBrush(color);

                var ellipse = new Ellipse() { Fill = brush, Height = 10, Width = 10 };

                if (Math.Abs(p.SubGroup) > 1)
                {
                    ellipse.Stroke = new SolidColorBrush(new ColorPicker().RevertColor(color));
                    ellipse.StrokeThickness = 2;
                }

                if (outliersY.Contains(p.Y))
                {
                    ellipse.Height = 20;
                    ellipse.Width = 20;
                }

                if (outliersX.Contains(p.X))
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

        private void DrawAxises()
        {
            var calculator = ScatterplotCalculator;

            double minX = calculator.MinX;
            double minY = calculator.MinX;
            double maxX = calculator.MaxX;
            double maxY = calculator.MaxY;

            ShowLable(_lableX, new Point()
            {
                X = CanvasCalculator.GetX((minX + maxX) / 2),
                Y = CanvasCalculator.GetY(minY)
            });

            ShowLable(_lableY, new Point()
            {
                X = CanvasCalculator.GetX(minX) - 15,
                Y = CanvasCalculator.GetY((minY + maxY) / 2)
            });

            var x1 = CanvasCalculator.GetX(minX);
            var x2 = x1;
            var y1 = CanvasCalculator.GetY(minY);
            var y2 = CanvasCalculator.GetY(maxY);

            var vLine = new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            mainCanvas.Children.Add(vLine);

            x1 = vLine.X1;
            y1 = vLine.Y2;
            x2 = vLine.X1 + 4;
            y2 = vLine.Y2 + 8;

            var triangleLine = new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            mainCanvas.Children.Add(triangleLine);

            x1 = vLine.X1;
            y1 = vLine.Y2;
            x2 = vLine.X1 - 4;
            y2 = vLine.Y2 + 8;

            triangleLine = new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            mainCanvas.Children.Add(triangleLine);

            x1 = CanvasCalculator.GetX(minX);
            x2 = CanvasCalculator.GetX(maxX);
            y1 = CanvasCalculator.GetY(minY);
            y2 = y1;

            var horizontalLine = new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            mainCanvas.Children.Add(horizontalLine);

        }

        private void Window_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            ResetCanvas();
            Draw();
        }

        private void ResetCanvas()
        {
            CanvasCalculator = null;
            ScatterplotCalculator = null;
            mainCanvas.Children.Clear();
            tbInfo.Text = String.Empty;
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
                    ShowText(_lableX + ": " + p.X + _lableY + ": " + p.Y, pt);
                }
            }
        }

        private void mainCanvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }


        private void ShowLable(string text, Point pt)
        {
            tb = new TextBlock
            {
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Text = text,
                Background = Brushes.WhiteSmoke
            };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            mainCanvas.Children.Add(tb);

            Canvas.SetLeft(tb, pt.X);
            Canvas.SetTop(tb, pt.Y);
        }

        private void ShowText(string text, Point pt)
        {
            tb = new TextBlock
            {
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Text = text,
                Background = Brushes.WhiteSmoke
            };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var xCoordinate = pt.X + 10;
            if (pt.X + tb.DesiredSize.Width > mainCanvas.ActualWidth)
            {
                xCoordinate = pt.X - tb.DesiredSize.Width - 10;
            }
            mainCanvas.Children.Add(tb);

            Canvas.SetLeft(tb, xCoordinate);
            Canvas.SetTop(tb, pt.Y - 10);
        }

        private void mainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition((Canvas)sender);

            if (tb != null)
            {
                mainCanvas.Children.Remove(tb);
            }
            tb = null;
        }

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            /* Загружает файл с точками. Формат строчек должен быть определенного вида: 
              первая строчка: имя первого параметра - имя второго  
              остальные: значение первого - значение второго.
              Разделитель пробел   */

            ScatterplotData data;

            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Text|*.txt|All|*.*",
                InitialDirectory = Directory.GetCurrentDirectory() + "\\" + "AppData"
            };

            if (dialog.ShowDialog() == true)
            {
                data = new ScatterplotFileLoader().Load(dialog.FileName);
            }
            else
            {
                return;
            }

            Init(data.LableX, data.LableY, new ScatterPointCollection(data.Points));

        }

        private void PrepareMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new BrentUsdAllDataPreparer();
            //new BrentUsdDataSeveralDaysAggregationPreparer();
            //new BrentUsdDataPreparer();
        }

        private void SettingsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow()
            {
                Title = "Settings",
                ShowInTaskbar = false,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
            };

            window.ShowDialog();
        }

        private void ShowResiduals_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new ResidualsWindow()
            {
                Title = "Residuals",
                ShowInTaskbar = true,
                Topmost = true,                
                Owner = Application.Current.MainWindow,
                Width = this.Width,
                Height = this.Height
            };

            window.Scatterplot.ShowZeroAxises = true;
            window.Scatterplot.Init(_points, null, _lableX, _lableY);

            window.ShowDialog();
        }

        private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || e.ClickCount != 2)
            {
                return;
            }

            Point pt = e.GetPosition((Canvas)sender);
            HitTestResult result = VisualTreeHelper.HitTest(mainCanvas, pt);

            if (result != null)
            {
                var v = result.VisualHit as Shape;
                if (v != null && v.DataContext != null)
                {
                    var p = (ScatterplotPoint)v.DataContext;
                    var index = _points.IndexOf(p);
                    if (index >= 0)
                    {
                        _points.RemoveAt(index);
                        ResetCanvas();
                        Draw();
                    }
                }
            }
        }

        private void CbDataFiles_OnSelected(object sender, RoutedEventArgs e)
        {
            var si = (KeyValuePair<string, string>)cbDataFiles.SelectedItem;
            var data = new ScatterplotFileLoader().Load(si.Value);
            Init(data.LableX, data.LableY, new ScatterPointCollection(data.Points));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var f = new ThreeSigmaOutliersFinder(_points.Select(p => p.Y));
            var outliersY = f.Find();

            f = new ThreeSigmaOutliersFinder(_points.Select(p => p.X));
            var outliersX = f.Find();

            _points = new ScatterPointCollection(_points.Where(p => outliersY.Contains(p.Y) == false));
            _points = new ScatterPointCollection(_points.Where(p => outliersX.Contains(p.X) == false));

            ResetCanvas();
            Draw();
        }

        private void DrawZeroAxises_OnClick(object sender, RoutedEventArgs e)
        {
            DrawZeroAxises(_points);    
            //if (cbDrawResiduals.IsChecked != null && cbDrawResiduals.IsChecked.Value)
            //{
            //    DrawZeroAxises(_residuals);    
            //}
            //else
            //{
                
            //}
            
        }

        private void DrawZeroAxises(ScatterPointCollection points)
        {
            if (ShowZeroAxises)
            {
                var calculator = new ScatterplotCalculator(points);

                if (calculator.MinX < 0 && calculator.MaxX > 0)
                {
                    var x1 = CanvasCalculator.GetX(0);
                    var x2 = x1;
                    var y1 = CanvasCalculator.GetY(calculator.MinY);
                    var y2 = CanvasCalculator.GetY(calculator.MaxY);

                    _zeroVerticalLine = new Line()
                    {
                        X1 = x1,
                        X2 = x2,
                        Y1 = y1,
                        Y2 = y2,
                        StrokeThickness = 0.5,
                        StrokeDashArray = new DoubleCollection(new Double[] { 2, 4 }),
                        Stroke = Brushes.Black
                    };

                    mainCanvas.Children.Add(_zeroVerticalLine);
                }

                if (calculator.MinY < 0 && calculator.MaxY > 0)
                {
                    var x1 = CanvasCalculator.GetX(calculator.MinX);
                    var x2 = CanvasCalculator.GetX(calculator.MaxX);
                    var y1 = CanvasCalculator.GetY(0);
                    var y2 = y1;

                    _zeroHorizontalLine = new Line()
                    {
                        X1 = x1,
                        X2 = x2,
                        Y1 = y1,
                        Y2 = y2,
                        StrokeThickness = 0.5,
                        StrokeDashArray = new DoubleCollection(new Double[] { 2, 4 }),
                        Stroke = Brushes.Black
                    };

                    mainCanvas.Children.Add(_zeroHorizontalLine);
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

        private void DrawResiduals_OnClick(object sender, RoutedEventArgs e)
        {
            ResetCanvas();

            //if (cbDrawResiduals.IsChecked != null && cbDrawResiduals.IsChecked.Value)
            //{
            //    _residuals = CalculateResiduals();
            //   // DrawAxises(_residuals);
            //    DrawPoints(_residuals, false);
            //}
            //else
            //{
            //    Draw();
            //}

        }

        private ScatterPointCollection CalculateResiduals()
        {
            var result = new List<ScatterplotPoint>();

            for (var categoryIndex = 0; categoryIndex < _points.GroupsCount; categoryIndex++)
            {
                var category = _points.Groups[categoryIndex];
                var categoryPoints = _points.Where(x => Math.Abs(x.Group) == category).ToArray();

                var regression = new LinearRegressionBuilder(categoryPoints.Select(x => x.X).ToArray(), categoryPoints.Select(x => x.Y).ToArray()).Build();

                foreach (var categoryPoint in categoryPoints)
                {
                    var y = categoryPoint.Y - (regression.a0 + regression.a1 * categoryPoint.X);
                    result.Add(new ScatterplotPoint()
                    {
                        Group = categoryPoint.Group,
                        Date = categoryPoint.Date,
                        X = categoryPoint.X,
                        Y = y,
                        SubGroup = categoryPoint.SubGroup
                    });                }

            }
            return new ScatterPointCollection(result);
        }        
    }
}
