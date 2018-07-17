using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Common.Classes.General;
using Visualizer.VisualControls.Controls;

namespace Visualizer.VisualControls.Classes
{
    public interface IStockChart
    {
        void Init(IEnumerable<Candle> candles);

        event EventHandler<StockChartMouseEventArgs> MouseUp;       
        event EventHandler<StockChartMouseEventArgs> MouseDown;       
        event EventHandler<StockChartMouseEventArgs> MouseMoved;       
        event EventHandler<StockChartRangeSelectedEventArgs> RangeSelected;       

        Canvas Canvas { get; }
        List<ChartMovingAverage> Emas { get; }
        List<ChartLine> ChartLines { get; }        

        ChartCalculator ChartCalculator { get; }
        void InitCustomDrawFunction(Action<Canvas, Candle> customDrawFunction);
        void InitCustomDrawRangeFunction(Action<Canvas, DateTime, DateTime> customDrawRangeFunction);
        void InitGetCandleDrawOptionsFunction(Func<Candle, CandleDrawOptions> getCandleDrawOptions);

        void Draw();
        void GoTo(int selectedIndex);
    }
}
