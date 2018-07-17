using System.Collections.Generic;
using Common.Classes.General;

namespace Common.Classes
{
    public interface ILongRangeDataProvider
    {
        void RecalculateDeviations(IEnumerable<Candle> candles);
        void RecalculateCandlesStatistics(IEnumerable<Candle> candles);

        CandlesStatistics GetCandlesStatistics();
        DistributionParameters GetDeviations(Direction direction);        
    }
}