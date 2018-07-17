using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Classes.StrategyTester
{
    /// <summary>
    /// Будет аккумулировать данные прогона робота
    /// </summary>
    /// <remarks>
    /// Существует ассиметрия между длинными и короткими сделками:
    /// Входим вверх по 10$, выходим по 20$, прибыль 10$ или 100%
    /// Входим вниз по 20$, выходим по 10$, прибыль 10$ или 50%    
    /// Пример:
    /// Входим вниз по 10$, выходим по 20$, убыток -10$, или (20$-10$)*100/10$ = -100%
    /// Входим вниз по 20$, выходим по 5$, прибыль 15$, или (20$-5$)*100/20$ = 75%
    /// Итого имеем: прибыль +5$ или -25%
    /// Предположим, что счет 100$, тогда -10$ это -10%, +15$ это +17%, итого имеем +5% или +7%
    /// </remarks>
    public class TradeDataAnalyzer
    {
       
        private readonly List<Deal> _deals;
        private List<ITradeItem> _tradeItems;
        private readonly double _startAmount;
        private readonly int _maxPositionSize;

        public TradeDataAnalyzer(IEnumerable<ITradeItem> allTradeItems, int maxPositionSize, double startAmount= 100000)
        {
            _tradeItems = allTradeItems.ToList();
            _startAmount = startAmount;
            _maxPositionSize = maxPositionSize;

            WritetoFile(_tradeItems);

            //ValidateIncorrectSize(_tradeItems);
            ValidateMinMaxSize(_tradeItems);

            _deals = FindDeals(_tradeItems);
        }

        private void WritetoFile(IReadOnlyList<ITradeItem> allTradeItems)
        {
            int size = 0;

            var fileName = "1.txt";
            if (File.Exists(fileName))
                fileName = "2.txt";

            //File.Create(fileName);

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                allTradeItems.Select(x => String.Join("\t", new[] { x.DateTime.ToString(), x.Price.ToString("N2"), x.Size.ToString() })).ToList().ForEach(sw.WriteLine);
            }            
        }

        private void ValidateIncorrectSize(IReadOnlyList<ITradeItem> allTradeItems)
        {
            double size = 0;

            for (int i = 0; i < allTradeItems.Count; i++)
            {
                var tradeSize = allTradeItems[i].Size;

                if ((size > 0 && (tradeSize + size) < 0) ||
                    (size < 0 && (tradeSize + size) > 0))
                {
                    throw new ArgumentException("Item: " + i + " has incorrect size");
                }

              
                size += allTradeItems[i].Size;
            }
        }


        private void ValidateMinMaxSize(IReadOnlyList<ITradeItem> allTradeItems)
        {
            double size = 0;
            
            for (int i = 0; i < allTradeItems.Count; i++)
            {
                var tradeSize = allTradeItems[i].Size;
              
                if (Math.Abs(tradeSize + size) >_maxPositionSize)
                {
                    throw new ArgumentException("Item: " + i + " leads to size that is more than maximum allowed");
                }


                size += allTradeItems[i].Size;
           }
        }

        private List<Deal> FindDeals(IReadOnlyList<ITradeItem> allTradeItems)
        {
            var result = new List<Deal>();

            int tradeItemIndex;
            var dealStartIndex = 0;

            do
            {
                var deal = new Deal();

                for (tradeItemIndex = dealStartIndex; tradeItemIndex < allTradeItems.Count; tradeItemIndex++)
                {
                    var tradeItem = allTradeItems[tradeItemIndex];

                    deal.TradeItems.Add(tradeItem);

                    if (deal.TradeItems.Count == 1)
                    {
                        deal.Direction = (tradeItem.Size > 0) ? Direction.Buy : Direction.Sell;
                    }
                    else if (deal.TradeItems.Sum(x => x.Size) == 0)
                    {
                        result.Add(deal);
                        dealStartIndex = tradeItemIndex + 1;
                        break;
                    }
                }

            } while (tradeItemIndex< allTradeItems.Count);

            return result;
        }

        public IEnumerable<Deal> Deals
        {
            get { return _deals; }
        }

        public TradeDataStatistics GetStatistics()
        {           
            var result = new TradeDataStatistics
            {
                Deals = _deals,

                NumberOfDealsAll = _deals.Count,
                NumberOfDealsLong = _deals.Count(x => x.Direction == Direction.Buy),
                NumberOfDealsShort = _deals.Count(x => x.Direction == Direction.Sell),

                NumberOfTradedContractsAll = _deals.Sum(x => x.TradeItems.Where(ti => ti.Size > 0).Sum(ti => ti.Size)),
                NumberOfTradedContractsLong = _deals.Where(x => x.Direction == Direction.Buy).Sum(x => x.TradeItems.Where(ti => ti.Size > 0).Sum(ti => ti.Size)),
                NumberOfTradedContractsShort = _deals.Where(x => x.Direction == Direction.Sell).Sum(x => x.TradeItems.Where(ti => ti.Size > 0).Sum(ti => ti.Size)),

                NumberOfProfitDealsAll = _deals.Count(x => x.GetAggregatedPercentProfit(_maxPositionSize)>0),
                NumberOfLossDealsAll = _deals.Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) <= 0),

                NumberOfProfitDealsLong = _deals.Where(x=>x.Direction == Direction.Buy).Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) > 0),
                NumberOfLossDealsLong = _deals.Where(x=>x.Direction == Direction.Buy).Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) <= 0),

                NumberOfProfitDealsShort = _deals.Where(x => x.Direction == Direction.Sell).Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) > 0),
                NumberOfLossDealsShort = _deals.Where(x => x.Direction == Direction.Sell).Count(x => x.GetAggregatedPercentProfit(_maxPositionSize) <= 0),

                MoneyProfitAll = _deals.Sum(x => x.GetProfitInMoney()),
                PercentAggregatedProfitAll = _deals.Sum(x => x.GetAggregatedPercentProfit(_maxPositionSize)),                

                MoneyProfitLong = _deals.Where(x=>x.Direction == Direction.Buy).Sum(x => x.GetProfitInMoney()),
                PercentAggregatedProfitLong = _deals.Where(x => x.Direction == Direction.Buy).Sum(x => x.GetAggregatedPercentProfit(_maxPositionSize)),

                MoneyProfitShort = _deals.Where(x => x.Direction == Direction.Sell).Sum(x => x.GetProfitInMoney()),
                PercentAggregatedProfitShort = _deals.Where(x => x.Direction == Direction.Sell).Sum(x => x.GetAggregatedPercentProfit(_maxPositionSize)),                
            };

            result.PercentProfitAll = result.MoneyProfitAll * 100 / _startAmount;
            result.PercentProfitLong = result.MoneyProfitLong * 100 / _startAmount;
            result.PercentProfitShort = result.MoneyProfitShort * 100 / _startAmount;
            
            result.MoneyDrawdownAll = CalculateMaximumDrawdown(_deals.Select(x => x.GetProfitInMoney()));
            result.PercentDrawdownAll = CalculateMaximumDrawdown(_deals.Select(x => x.GetAggregatedPercentProfit(_maxPositionSize)));

            result.MoneyDrawdownLong = CalculateMaximumDrawdown(_deals.Where(x=>x.Direction == Direction.Buy).Select(x => x.GetProfitInMoney()));
            result.PercentDrawdownLong = CalculateMaximumDrawdown(_deals.Where(x=>x.Direction == Direction.Buy).Select(x => x.GetAggregatedPercentProfit(_maxPositionSize)));

            result.MoneyDrawdownShort = CalculateMaximumDrawdown(_deals.Where(x => x.Direction == Direction.Sell).Select(x => x.GetProfitInMoney()));
            result.PercentDrawdownShort = CalculateMaximumDrawdown(_deals.Where(x => x.Direction == Direction.Sell).Select(x => x.GetAggregatedPercentProfit(_maxPositionSize)));

            if (_tradeItems.Any())
            {
                result.MoneyProfitMarket = (_tradeItems.Last().Price - _tradeItems.First().Price);
                result.PercentProfitMarket = (_tradeItems.Last().Price - _tradeItems.First().Price) * 100 / _tradeItems.First().Price;
            }
            return result;
        }

        private double CalculateMaximumDrawdown(IEnumerable<double> values )
        {
            return CalculateMaximumDrawdown(values.ToArray());
        }

     public static double CalculateMaximumDrawdown(double[] values)
        {
            var drawdowns = new List<double>();

            double currentLevel = 0.0;
            double cumulativeProfit = 0.0;
            double minCumulativeProfit = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];

                cumulativeProfit += value;

                if (cumulativeProfit - currentLevel < minCumulativeProfit)
                {
                    minCumulativeProfit = cumulativeProfit - currentLevel;
                }

                if (cumulativeProfit > currentLevel)
                {
                    drawdowns.Add(minCumulativeProfit);
                    currentLevel = cumulativeProfit;
                }
            }

            drawdowns.Add(minCumulativeProfit);

            return drawdowns.Min();
        }
    }
}
