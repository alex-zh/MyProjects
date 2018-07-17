using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Robot.DataLayer.Repositories;

namespace Robot.Core.Classes
{
    /// <summary>
    /// Note this updator can take long time (till 20 seconds) to complete work. 
    /// Must be launched in separate thread.
    /// </summary>
    public class CompletedTradesUpdator
    {
        /// <summary>
        /// How much time we wait trades to appear in Quik
        /// </summary>
        private const int WaitCompletedTradesTimeoutInSeconds = 30;
        private const int WaitCompletedTradesOffsetInMiliseconds = 200;

        private readonly RobotNames _robotName;
        private readonly ILogger _logger;
        private readonly Func<IEnumerable<RobotTrade>> _getQuikTrades;
        private readonly RobotTradeRepository _tradesRepository = new RobotTradeRepository();

        public CompletedTradesUpdator(RobotNames robotName, ILogger logger, Func<IEnumerable<RobotTrade>> getQuikTrades)
        {
            _robotName = robotName;
            _logger = logger;
            _getQuikTrades = getQuikTrades;
        }

        public void Update(LimitedOrder order)
        {
            var requiredSize = order.Params.Size - order.Balance;

            var startTime = DateTime.Now;
            var trades = new List<RobotTrade>();

            while ((DateTime.Now - startTime).TotalSeconds < WaitCompletedTradesTimeoutInSeconds)
            {
                Thread.Sleep(WaitCompletedTradesOffsetInMiliseconds);

                trades = CollectCompletedTrades(order.OrderNumber, requiredSize);

                if (trades.Any())
                {
                    break;
                }
            }

            if (trades.Any())
            {
                UpdateTradesInDatabase(trades);
            }
            else
            {
                _logger.Log(String.Format("Trade for order {0} was not found. It will lead to miss in trades table", order.OrderNumber), LogMessageTypes.Error);
            }
        }

        private List<RobotTrade> CollectCompletedTrades(long orderNumber, int requiredSize)
        {
            return new CompletedTradesCollector().Collect(_getQuikTrades().Where(x => x.OrderNumber == orderNumber), requiredSize);
        }

        private void UpdateTradesInDatabase(IEnumerable<RobotTrade> trades)
        {
            var dbTrades = _tradesRepository.GetAllFor(_robotName).ToList();
            var newTrades = trades.Where(x => dbTrades.Count(dbTrade => dbTrade.TradeNumber == x.TradeNumber) == 0);

            _tradesRepository.Add(_robotName, newTrades);
        }
    }
}
