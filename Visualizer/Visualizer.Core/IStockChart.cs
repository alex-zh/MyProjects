using System.Collections.Generic;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Visualizer.Core
{
    public interface IStockChart
    {
        void Init(IEnumerable<Candle> candles);        
    }
}
