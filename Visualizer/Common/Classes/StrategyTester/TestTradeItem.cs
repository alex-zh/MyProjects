using System;

namespace Common.Classes.StrategyTester
{
    public class TestTradeItem : ITradeItem
    {
        public double Size { get; set; }        

        public DateTime DateTime { get; set; }        

        public double Price { get; set; }        
    }
}