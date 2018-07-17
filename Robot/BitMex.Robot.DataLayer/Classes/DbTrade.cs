using System;

namespace BitMex.Robot.DataLayer.Classes
{
    public class DbTrade
    {
        public string TradeId { get; set; }

        public string RobotId { get; set; }

        // Buy = 0, Sell = 1 
        public int Side { get; set; }

        public double Size { get; set; }

        public double Price { get; set; }

        public string Symbol { get; set; }

        public string OrderId { get; set; }

        public string SystemOrderId { get; set; }

        public DateTime DateTime { get; set; }        
    }
}