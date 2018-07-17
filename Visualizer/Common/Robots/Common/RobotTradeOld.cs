using System;
using Common.Classes.StrategyTester;

namespace Common.Robots.Common
{    
    public class RobotTradeOld: ITradeItem
    {
        public int Id { get; set; }
        
        public double Size { get; set; }

        public double Price { get; set; }

        public DateTime DateTime { get; set; }

        /// <summary>
        /// Trade number in Quik
        /// </summary>
        public long TradeNumber { get; set; }

        /// <summary>
        /// Corresponding order number in Quik
        /// </summary>
        public long OrderNumber { get; set; }

        /// <summary>
        /// Представляет тикер актива (акции, фьючерса), по которому совершена операция
        /// </summary>
        public string Security { get; set; }
        
    }
}