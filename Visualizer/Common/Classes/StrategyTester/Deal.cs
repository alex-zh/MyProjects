using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.StrategyTester
{
    /// <summary>
    /// ѕредставл€ет набор индивидуальных трейдов, представл€ющий одну сделку.
    /// Ќапример, покупка 2 контрактов, потом еще 2-х и продажа 4-х будет представл€ть одну завершенную сделку на покупку
    /// </summary>
    public class Deal
    {
        private List<ITradeItem> _dealTradeItems = new List<ITradeItem>();

        public Direction Direction { get; set; }

        public List<ITradeItem> TradeItems
        {
            get { return _dealTradeItems; }
            set { _dealTradeItems = value; }
        }

        public double InitialSize
        {
            get { return _dealTradeItems.First().Size; }
        }

        public DateTime StartDate
        {
            get { return _dealTradeItems.First().DateTime; }
        }

        public DateTime EndDate
        {
            get { return _dealTradeItems.Last().DateTime; }
            
        }

        public double InitialPrice
        {
            get { return _dealTradeItems.First().Price; }
        }

        public double GetProfitInMoney()
        {
            var upPrice = _dealTradeItems.Where(x => x.Size > 0).Sum(x => Math.Abs(x.Size) * x.Price);
            var downPrice = _dealTradeItems.Where(x => x.Size < 0).Sum(x => Math.Abs(x.Size) * x.Price);

            switch (Direction)
            {
                case Direction.Buy:
                    return (downPrice - upPrice);
                case Direction.Sell:
                    return (downPrice - upPrice);
                default:
                    return 0;
            }
        }

        public double GetPercentProfit()
        {
            var initialPrice = _dealTradeItems.First().Price;
            var initialSize = Math.Abs(_dealTradeItems.First().Size);

            return GetProfitInMoney() * 100 / (initialPrice*initialSize);
        }

        public double GetAggregatedPercentProfit(int maxPositionSize)
        {
            var upTrades = _dealTradeItems.Where(x => x.Size > 0).ToList();
            var downTrades = _dealTradeItems.Where(x => x.Size < 0).ToList();

            var upAveragePrice = upTrades.Sum(x => x.Price * x.Size) / upTrades.Sum(x => x.Size);
            var downAveragePrice = downTrades.Sum(x => x.Price * x.Size) / downTrades.Sum(x => x.Size);

            var size = (upTrades.Sum(x => x.Size));
            var normalizeCoefficient = ((double)size)/maxPositionSize;

            switch (Direction)
            {
                case Direction.Buy:
                    return normalizeCoefficient * (downAveragePrice - upAveragePrice) * 100 / upAveragePrice;
                case Direction.Sell:
                    return normalizeCoefficient * (downAveragePrice - upAveragePrice) * 100 / downAveragePrice;
                default:
                    return 0;
            }
        }
    }
}