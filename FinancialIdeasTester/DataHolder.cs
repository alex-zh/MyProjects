using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Classes;


namespace FinancialIdeasTester
{
    public class DataHolder
    {
        public DataHolder()
        {
            Candles = new List<Candle>();
            Changes = new List<CandleChange>();
        }

        public List<Candle> Candles { get; private set; }
        public List<CandleChange> Changes { get;  set; }
    }
}
