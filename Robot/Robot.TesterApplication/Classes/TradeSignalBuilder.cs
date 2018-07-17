using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Classes.General;
using Robot.TesterApplication.Enums;
using Visualizer.VisualControls.Classes;

namespace Robot.TesterApplication.Classes
{
    public class TradeSignalBuilder
    {
        private readonly IStockChart _chart;

        public TradeSignalBuilder(IStockChart chart)
        {
            _chart = chart;
        }

        /// <summary>
        /// Сигналы на покупку и продажу рисуются под свечей
        /// Сингналы на закрытие рисуются над свечей
        /// singalOrder means number of singal, if there are 2 singnals for one date first will have order 0, second - 1 
        /// </summary>
        public FrameworkElement BuildTradeSignal(Candle candle, TradeSignal signal, int singalOrder = 0)
        {
            double x = _chart.ChartCalculator.GetCoordinateByDate(candle.Date);
            double y = _chart.ChartCalculator.GetCoordinateByValue(signal.TradeSignalType == TradeSignalTypes.Close ? candle.High : candle.Low);

            var brushColor = GetSignalColor(signal);
            var signalOrderBuysellOffset = singalOrder * (13 + 2);
            var signalOrderCloseOffset = singalOrder * (-6 - 2);

            var plg = new Polygon
            {
                Stroke = brushColor,
                StrokeThickness = 1,
                Fill = brushColor,
                DataContext = signal
            };

            if (signal.TradeSignalType == TradeSignalTypes.Close)
            {
                plg.Points.Add(new Point(x - 5, y - 4 + signalOrderCloseOffset));
                plg.Points.Add(new Point(x - 5, y - 6 + signalOrderCloseOffset));
                plg.Points.Add(new Point(x + 5, y - 6 + signalOrderCloseOffset));
                plg.Points.Add(new Point(x + 5, y - 4 + signalOrderCloseOffset));
                plg.Points.Add(new Point(x - 5, y - 4 + signalOrderCloseOffset));
            }
            else if (signal.TradeSignalType == TradeSignalTypes.Buy)
            {
                plg.Points.Add(new Point(x - 5, y + 13 + signalOrderBuysellOffset));
                plg.Points.Add(new Point(x, y + 3 + signalOrderBuysellOffset));
                plg.Points.Add(new Point(x + 5, y + 13 + signalOrderBuysellOffset));
                plg.Points.Add(new Point(x - 5, y + 13 + signalOrderBuysellOffset));
            }
            else if (signal.TradeSignalType == TradeSignalTypes.Sell)
            {
                plg.Points.Add(new Point(x - 5, y + 3 + signalOrderBuysellOffset));
                plg.Points.Add(new Point(x, y + 13 + signalOrderBuysellOffset));
                plg.Points.Add(new Point(x + 5, y + 3 + signalOrderBuysellOffset));
                plg.Points.Add(new Point(x - 5, y + 3 + signalOrderBuysellOffset));
            }

            return plg;
        }

        private static Brush GetSignalColor(TradeSignal signal)
        {
            switch (signal.TradeSignalType)
            {
                case TradeSignalTypes.Buy:
                    return Brushes.Green;
                case TradeSignalTypes.Sell:
                    return Brushes.Red;
                case TradeSignalTypes.Close:
                    return Brushes.Blue;
            }

            return Brushes.Black;
        }

        public FrameworkElement BuildCircle(Candle candle, Brush brush)
        {
            var brushColor = brush;

            var ellipse = new Ellipse()
            {
                Stroke = brushColor,
                StrokeThickness = 1,
                Fill = brushColor,
                Width = 10,
                Height = 10,
            };

            return ellipse;
        }

        public IEnumerable<Line> BuildLines(TradeSignal tradeSignal)
        {
            var y = _chart.ChartCalculator.GetCoordinateByValue(tradeSignal.Price);
            var x1 = _chart.ChartCalculator.GetCoordinateOfFirstVisibleCandle();
            var x2 = _chart.ChartCalculator.GetCoordinateOfLastVisibleCandle();

            var line = new Line
            {
                Stroke = GetSignalColor(tradeSignal),
                StrokeThickness = 1,
                X1 = x1,
                Y1 = y,
                X2 = x2,
                Y2 = y
            };

            return new[] { line };
        }
    }
}