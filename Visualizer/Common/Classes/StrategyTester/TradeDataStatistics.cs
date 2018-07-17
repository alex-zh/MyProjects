using System;
using System.Collections.Generic;

namespace Common.Classes.StrategyTester
{
    /// <summary>
    /// Drawdown это просадка счета между двумя последовательными вершинами, 
    /// при этом вторая вершина выше предыдущей
    /// </summary>
    [Serializable]
    public class TradeDataStatistics
    {
        public TradeDataStatistics()
        {
            Deals = new List<Deal>();
        }

        public List<Deal> Deals { get; set; } 

        public double MoneyProfitAll { get; set; }
        public double PercentProfitAll { get; set; }
        public double PercentAggregatedProfitAll { get; set; }

        public double MoneyProfitLong { get; set; }
        public double PercentProfitLong { get; set; }
        public double PercentAggregatedProfitLong { get; set; }

        public double MoneyProfitShort { get; set; }
        public double PercentProfitShort { get; set; }
        public double PercentAggregatedProfitShort { get; set; }

        public double MoneyProfitMarket { get; set; }
        public double PercentProfitMarket { get; set; }

        public int NumberOfDealsAll { get; set; }
        public int NumberOfDealsLong { get; set; }
        public int NumberOfDealsShort { get; set; }

        public double NumberOfTradedContractsAll { get; set; }
        public double NumberOfTradedContractsLong { get; set; }
        public double NumberOfTradedContractsShort { get; set; }

        public double MoneyDrawdownAll { get; set; }
        public double PercentDrawdownAll { get; set; }

        public double MoneyDrawdownLong { get; set; }
        public double PercentDrawdownLong { get; set; }

        public double MoneyDrawdownShort { get; set; }
        public double PercentDrawdownShort { get; set; }

        public int NumberOfProfitDealsAll { get; set; }
        public int NumberOfLossDealsAll { get; set; }
        public int NumberOfProfitDealsLong { get; set; }
        public int NumberOfLossDealsLong { get; set; }
        public int NumberOfProfitDealsShort { get; set; }
        public int NumberOfLossDealsShort { get; set; }
        
    }
}