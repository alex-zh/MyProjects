using System;
using System.Linq;
using Common.Classes.Logger;
using Common.DataLayer.Repositories;
using Common.Robots.Common;
using Robot.Core.MarketDataProviders;

namespace Robot.Core.Classes
{
    /// <summary>
    /// Note that single order can be executed in several trades in case of low liquidity 
    /// Note this updator can take long time (till 20 seconds) to complete work. 
    /// Must be launched in separate thread.
    /// </summary>
    public class TradesUpdator
    {
        private readonly string _robotId;
        private readonly IRobotRepository _robotRepository;
        private readonly IRobotTradeRepository _tradesRepository;
        private readonly IMarketDataProvider _dataProvider;
        private readonly ILogger _logger;

        public TradesUpdator(string robotId, 
                             IRobotRepository robotRepository, 
                             IRobotTradeRepository tradesRepository, 
                             IMarketDataProvider dataProvider, 
                             ILogger logger)
        {
            _robotId = robotId;
            _robotRepository = robotRepository;
            _tradesRepository = tradesRepository;
            _dataProvider = dataProvider;
            _logger = logger;
        }

        public void Update()
        {
            var orders = _robotRepository.GetStateByRobotId(_robotId).GetOrders();

            foreach (var order in orders)
            {
                UpdateOrder(order);
            }
        }

        private void UpdateOrder(LimitOrder order)
        {
            var trades = _tradesRepository.GetAll(_robotId, order.OrderId).ToList();

            var tradesBalance = trades.Sum(x => x.Size);

            if (Math.Abs(order.Balance - tradesBalance) > double.Epsilon)
            {                
                var marketTrades = _dataProvider.GetTrades(order.OrderId);

                _tradesRepository.RemoveAll(_robotId, order.OrderId);
                _tradesRepository.Add(_robotId, marketTrades);

                _logger.Info($"Trades updated for order {order.OrderId}. " +
                             $"Order balance is {order.Balance} while trades balance is {tradesBalance}");
            }
        }

    }
}
