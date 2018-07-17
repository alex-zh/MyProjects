using System;

namespace Common.Classes.StrategyTester
{
    public interface ITradeItem
    {
        double Size { get; set; }

        DateTime DateTime { get; set; }

        double Price { get; set; }      
    }
}