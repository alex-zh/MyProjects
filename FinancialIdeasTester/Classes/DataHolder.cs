using System.Collections.Generic;

namespace FinancialIdeasTester.Classes
{
    public class DataHolder
    {
        public DataHolder()
        {
            Candles = new List<Candle>();
            Changes = new List<CandleChange>();
            DetrenedValues = new List<DetrendedValue>();
        }

        public List<Candle> Candles { get; private set; }
        public List<DetrendedValue> DetrenedValues { get; private set; }
        public List<CandleChange> Changes { get;  set; }
    }
}
