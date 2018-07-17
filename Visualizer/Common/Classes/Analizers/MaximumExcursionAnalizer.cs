using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.StrategyTester;

namespace Common.Classes.Analizers
{
    /// <summary>
    /// Находит информацию для MFE и MAE
    /// Допущения:
    /// 1. Вход в сделку сразу происходит на всю возможную позиции
    /// </summary>
    public class MaximumExcursionAnalizer
    {
        private readonly List<Deal> _deals;
        private readonly List<Candle> _candles;

        public MaximumExcursionAnalizer(List<Deal> deals, List<Candle> candles)
        {
            _deals = deals;
            _candles = candles;
        }

        public List<MaximumExcursionPoint> GetMfe()
        {
            return _deals.AsParallel().Select(GetMaximumFavorableExcursionForDeal).ToList();
        }

        public List<MaximumExcursionPoint> GetMae()
        {
            return _deals.AsParallel().Select(GetMaximumAdverseExcursionForDeal).ToList();
        }

        private MaximumExcursionPoint GetMaximumFavorableExcursionForDeal(Deal deal)
        {
            var result = new MaximumExcursionPoint();

            var startPrice = deal.InitialPrice;
            var startIndex = _candles.GetIndexByDate(deal.StartDate);
            var endIndex = _candles.GetIndexByDate(deal.EndDate); //выход происходит на конечной свече

            result.RealizedValue = deal.GetPercentProfit();

            if (deal.Direction == Direction.Buy)
            {
                var maximumValue = _candles.IndexRange(startIndex, endIndex).AsParallel().Max(x => x.High);

                result.MaximumValue = (maximumValue - startPrice) * 100 / startPrice;
            }
            else if (deal.Direction == Direction.Sell)
            {
                var minimumValue = _candles.IndexRange(startIndex, endIndex).AsParallel().Min(x => x.Low);

                result.MaximumValue = (startPrice - minimumValue) * 100 / startPrice;
            }

            return result;
        }

        private MaximumExcursionPoint GetMaximumAdverseExcursionForDeal(Deal deal)
        {
            var result = new MaximumExcursionPoint();

            var startPrice = deal.InitialPrice;
            var startIndex = _candles.GetIndexByDate(deal.StartDate);
            var endIndex = _candles.GetIndexByDate(deal.EndDate);

            result.RealizedValue = deal.GetPercentProfit();

            if (deal.Direction == Direction.Buy)
            {
                var minimumValue = _candles.IndexRange(startIndex, endIndex).AsParallel().Min(x => x.Low);

                result.MaximumValue = (startPrice - minimumValue) * 100 / startPrice;
            }
            else if (deal.Direction == Direction.Sell)
            {
                var maximumValue = _candles.IndexRange(startIndex, endIndex).AsParallel().Max(x => x.High);

                result.MaximumValue = (maximumValue - startPrice) * 100 / startPrice;
            }

            return result;
        }
    }
}
