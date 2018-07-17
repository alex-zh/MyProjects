using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;

namespace Robot.Core.Classes
{
    /// <summary>
    /// When order is partially completed
    /// we need to collect trades from quik to get information about prices the order was executed at
    /// </summary>
    /// <remarks>
    /// 1. Trades can appear in quik with a delay
    /// 2. If order is partially completed we need to collect all trades composing (size - balance) amount
    /// </remarks>
    public class CompletedTradesCollector
    {
        public List<RobotTrade> Collect(IEnumerable<RobotTrade> availableTrades, int requiredCount)
        {
            var result = new List<RobotTrade>();
            var sortedTrades = availableTrades.OrderBy(x => x.Date).ThenBy(x => x.TradeNumber).ToList();

            foreach (var trade in sortedTrades)
            {
                result.Add(trade);

                if (result.Sum(x => Math.Abs(x.Size)) == requiredCount)
                {
                    return result;
                }
            }

            return new List<RobotTrade>();
        }
    }
}
