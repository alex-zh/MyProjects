using System;

namespace Robot.DataLayer.Classes
{
    /// <summary>
    /// Представляет совершенную операцию
    /// </summary>
    public class DbRobotTrade
    {
        public int Id { get; set; }

        public int RobotId { get; set; }        

        public int Size { get; set; }

        public double Price { get; set; }

        public long TradeNumber { get; set; }

        public long OrderNumber { get; set; }

        public DateTime DateTime { get; set; }        
    }
}