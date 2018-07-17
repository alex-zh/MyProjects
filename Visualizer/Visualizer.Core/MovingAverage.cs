using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Visualizer.Core
{
    public class MovingAverage
    {
        private readonly int _period;
        private readonly List<DatePrice> _ema;

        public MovingAverage(IEnumerable<Candle> candles, int period, PriceTypes priceType)            
        {
            _period = period;
            _ema = candles.Ema(_period, priceType).ToList();           
        }
        
        public DatePrice this[int index]
        {
            get { return _ema[index]; }
        }

        public int Period { get { return _period; } }
    }
}