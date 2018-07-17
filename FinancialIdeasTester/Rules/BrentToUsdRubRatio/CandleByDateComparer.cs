using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester.Rules.BrentToUsdRubRatio
{
    class CandleByDateComparer : IEqualityComparer<Candle>
    {
        public bool Equals(Candle x, Candle y)
        {
            return x.Date == y.Date;
        }

        public int GetHashCode(Candle obj)
        {
            return obj.Date.GetHashCode();
        }
    }
}
