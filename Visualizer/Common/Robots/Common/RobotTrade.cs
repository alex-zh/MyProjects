using System;
using Common.Classes.StrategyTester;

namespace Common.Robots.Common
{    
    public class RobotTrade: ITradeItem
    {
        public double Size { get; set; }

        public OrderSides Side { get; set; }

        public double Price { get; set; }

        public DateTime DateTime { get; set; }

        public string TradeId { get; set; }

        public string OrderId { get; set; }

        public string Symbol { get; set; }
        
    }
}