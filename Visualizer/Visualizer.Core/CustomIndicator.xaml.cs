using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Classes;
using Common.Classes.General;
using Core;
using Visualizer.Core.Classes;

namespace Visualizer.Core
{
    public partial class CustomIndicator
    {
        private ChartCalculator _chartCalculator;

        public CustomIndicator()
        {
            InitializeComponent();            
        }               

        public void Init(ChartCalculator calculator, GroupingInterval groupingInterval)
        {            
            _chartCalculator = calculator;
            canvas.Height = grid.ActualHeight;

            DrawIndicator();
        }

        private void DrawIndicator()
        {
            canvas.Children.Clear();
            
            var candles = _chartCalculator.Candles.ToArray();

            var volatiltiyCalculator = new CustomVolatilityCalculator(candles, 11);            
            
            AddLevels(candles);
            AddIndicator(candles, volatiltiyCalculator);
            AddIndicatorEMA(candles, volatiltiyCalculator);
        }

        private void AddLevels(Candle[] candles)
        {
            Line line;
            SolidColorBrush redBrush;
            
            redBrush = new SolidColorBrush(Colors.Red);
            line = new Line();
            line.Stroke = redBrush;
            line.X1 = 0;
            line.X2 = _chartCalculator.GetCoordinateByDate(candles[candles.Length - 1].Date, true);
            line.Y1 = GetCoordinateByValue(30, 0, 100);
            line.Y2 = line.Y1;

            canvas.Children.Add(line);

            line = new Line();
            line.Stroke = redBrush;
            line.X1 = 0;
            line.X2 = _chartCalculator.GetCoordinateByDate(candles[candles.Length - 1].Date, true);
            line.Y1 = GetCoordinateByValue(60, 0, 100);
            line.Y2 = line.Y1;

            canvas.Children.Add(line);
        }

        private void AddIndicator(Candle[] candles, CustomVolatilityCalculator volatiltiyCalculator)
        {            
            PointCollection points = new PointCollection();

            for (int i = 0; i < candles.Length; i++)
            {
                var x = _chartCalculator.GetCoordinateByDate(candles[i].Date, true);
                var y = GetCoordinateByValue(volatiltiyCalculator[i], 0, 100);

                points.Add(new Point(x, y));
            }

            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;

            // Create a polyline
            Polyline indicatorPolyline = new Polyline();
            indicatorPolyline.Stroke = blackBrush;
            indicatorPolyline.StrokeThickness = 1;

            indicatorPolyline.Points = points;

            // Add polyline to the page
            canvas.Children.Add(indicatorPolyline);        
        }

        private void AddIndicatorEMA(Candle[] candles, CustomVolatilityCalculator volatiltiyCalculator)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < candles.Length; i++)
            {
                var x = _chartCalculator.GetCoordinateByDate(candles[i].Date, true);
                var y = GetCoordinateByValue(volatiltiyCalculator.GetEma(i), 0, 100);

                points.Add(new Point(x, y));
            }

            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.Green;

            // Create a polyline
            Polyline polyline = new Polyline();
            polyline.Stroke = brush;
            polyline.StrokeThickness = 2;

            polyline.Points = points;

            // Add polyline to the page
            canvas.Children.Add(polyline);
        }

        public double CanvasWidth
        {
            get { return canvas.Width; }
            set { canvas.Width = value; }
        }

        public double GetCoordinateByValue(double value, double minValue, double maxValue)
        {
            return canvas.ActualHeight - (value - minValue) * canvas.ActualHeight / (maxValue - minValue);
        }

    }
}
