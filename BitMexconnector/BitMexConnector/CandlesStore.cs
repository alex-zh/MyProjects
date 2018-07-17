using System.Collections.Generic;
using Common.Classes;
using Common.Classes.General;

namespace BitMexConnector
{
    public class CandlesStore
    {
        private readonly Dictionary<TimeFrames, List<Candle>> _candles = new Dictionary<TimeFrames, List<Candle>>();

        public List<Candle> Get(TimeFrames timeFrame)
        {
            if(_candles.ContainsKey(timeFrame))
                return _candles[timeFrame];

            return  new List<Candle>();
        } 
    }
}