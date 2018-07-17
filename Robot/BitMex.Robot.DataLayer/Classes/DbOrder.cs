using System;

namespace BitMex.Robot.DataLayer.Classes
{
    public class DbOrder
    {
        public string OrderId { get; set; }

        public string SystemOrderId { get; set; }

        public string RobotId { get; set; }

        public string Symbol { get; set; }

        // New / Canceled
        public int Status { get; set; }

        // Buy = 0, Sell = 1 
        public int OrderSide { get; set; }

        public double Size { get; set; }

        public double Balance { get; set; }

        public double Price { get; set; }

        public DateTime DateTime { get; set; }

        // Buy / Sell
        public int Side { get; set; }        
    }
}