using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Classes.StrategyTester
{
    public class DealsAnalyzer
    {
        private readonly List<Deal> _deals;
        private readonly int _maxPositionSize;

        private readonly double _allDealsDeviation;
        private readonly double _longDealsDeviation;
        private readonly double _shortDealsDeviation;

        public DealsAnalyzer(List<Deal> deals, int maxPositionSize)
        {
            _deals = deals;
            _maxPositionSize = maxPositionSize;
            _allDealsDeviation = new StatisticsCalculator(_deals.Select(x => x.GetAggregatedPercentProfit(_maxPositionSize))).Sigma();
            _longDealsDeviation = new StatisticsCalculator(_deals.Where(x=>x.Direction == Direction.Buy).Select(x => x.GetAggregatedPercentProfit(_maxPositionSize))).Sigma();
            _shortDealsDeviation = new StatisticsCalculator(_deals.Where(x => x.Direction == Direction.Sell).Select(x => x.GetAggregatedPercentProfit(_maxPositionSize))).Sigma();
        }

        public int GetAllProfitableDealsNumber(double proifitFactor)
        {
            return _deals.Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) >= _allDealsDeviation*proifitFactor);
        }

        public int GetAllDealsNumber(double proifitFactor)
        {
            return _deals.Count(x => Math.Abs(x.GetAggregatedPercentProfit(_maxPositionSize)) >= _allDealsDeviation * proifitFactor);
        }

        public int GetLongProfitableDealsNumber(double proifitFactor)
        {
            return _deals.Where(x=>x.Direction == Direction.Buy).Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) >= _longDealsDeviation * proifitFactor);
        }

        public int GetLongDealsNumber(double proifitFactor)
        {
            return _deals.Where(x => x.Direction == Direction.Buy).Count(x => Math.Abs(x.GetAggregatedPercentProfit(_maxPositionSize)) >= _longDealsDeviation * proifitFactor);
        }

        public int GetShortProfitableDealsNumber(double proifitFactor)
        {
            return _deals.Where(x => x.Direction == Direction.Sell).Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) >= _shortDealsDeviation * proifitFactor);
        }

        public int GetShortDealsNumber(double proifitFactor)
        {
            return _deals.Where(x => x.Direction == Direction.Sell).Count(x => Math.Abs(x.GetAggregatedPercentProfit(_maxPositionSize)) >= _shortDealsDeviation * proifitFactor);
        }
    }
}
