using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.StrategiesOptions;

namespace Common.Classes
{
    public class LongRangeDataProvider : BaseLongTermParametersProvider, ILongRangeDataProvider
    {
        private readonly StrategyOptions _options;

        public LongRangeDataProvider(StrategyOptions options, ILogger logger): base(logger)
        {
            _options = options;
            FmaPeriod = _options.FmaPeriod;
            SmaPeriod = _options.SmaPeriod;
        }

        public void RecalculateDeviations(IEnumerable<Candle> candles)
        {
            _candlesForDeviations = candles.ToList();

            RecalculateDeviations();
        }

        public void RecalculateIntersections(IEnumerable<Candle> candles)
        {
            _candlesForIntersections = candles.ToList();
            
            RecalculateIntersections();
        }

        public void RecalculateCandlesStatistics(IEnumerable<Candle> candles)
        {
            _candlesForStatistics = candles.ToList();
            RecalculateCandlesStatistics();
        }
    }
}
