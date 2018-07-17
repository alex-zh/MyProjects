using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Logger;

namespace Common.Classes
{
    public class TestLongRangeDataProvider : BaseLongTermParametersProvider, ILongRangeDataProvider
    {
        private List<Candle> _allHistoryCandles;

        public TestLongRangeDataProvider(List<Candle> candles, int fmaPeriod, int smaPeriod, ILogger logger)
            : base(logger)
        {
            _allHistoryCandles = candles;

            FmaPeriod = fmaPeriod;
            SmaPeriod = smaPeriod;
        }

        public void RecalculateDeviations(IEnumerable<Candle> candles)
        {
            var lastCandle = candles.Last();
            var lastCandleHistoryIndex = _allHistoryCandles.GetIndexByDate(lastCandle.Date);
            var firstCandleHistoryIndex = Math.Max(0, lastCandleHistoryIndex - RangeLength);

            _candlesForDeviations = _allHistoryCandles.IndexRange(firstCandleHistoryIndex, lastCandleHistoryIndex).ToList();

            RecalculateDeviations();            
        }

        public void RecalculateIntersections(IEnumerable<Candle> candles)
        {
            var lastCandle = candles.Last();
            var lastCandleHistoryIndex = _allHistoryCandles.GetIndexByDate(lastCandle.Date);
            var firstCandleHistoryIndex = Math.Max(0, lastCandleHistoryIndex - RangeLength);

            _candlesForIntersections = _allHistoryCandles.IndexRange(firstCandleHistoryIndex, lastCandleHistoryIndex).Where(x => x.Date.Day == lastCandle.Date.Day).ToList();

            RecalculateIntersections();
        }

        public void RecalculateCandlesStatistics(IEnumerable<Candle> candles)
        {
            _candlesForStatistics = candles.ToList();
            RecalculateCandlesStatistics();
        }
    }
}