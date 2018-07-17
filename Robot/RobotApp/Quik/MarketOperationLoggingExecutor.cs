using System;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Core.Providers;
using Robot.Core.Providers.Quik;

namespace Robot.Core.OperationExecutors
{
    public class MarketOperationLoggingExecutor : IMarketOperationExecutor
    {        
        private static int _orderNumber = 0;
        private readonly QuikLoggingMarketDataProvider _loggingMarketDataProvider;
        private readonly ILogger _logger;
        
        public MarketOperationLoggingExecutor(QuikLoggingMarketDataProvider loggingMarketDataProvider, ILogger logger)
        {
            _loggingMarketDataProvider = loggingMarketDataProvider;
            _logger = logger;
        }

        public LimitOrder CreateOrder(Securities security, OrderParams orderParams)
        {
            var order = new LimitOrder
            {                
                Params = orderParams,
                DateTime = DateTime.Now,
                Balance = orderParams.Size,
                OrderNumber = (_orderNumber++),                                
                Status =  OrderStatuses.Active,
                Security = security
            };

            if (Math.Abs(orderParams.Price) < Double.Epsilon)
            {
                var lastCandle = _loggingMarketDataProvider.GetLastCandle(security);

                order.Params.Price = lastCandle.Close;
            }

            _loggingMarketDataProvider.AddOrder(order);

            return order;
        }

        public bool KillOrder(Securities security, long orderId)
        {
            var order = new LimitOrder
            {
                DateTime = DateTime.Now,
                OrderNumber = orderId,
                Status = OrderStatuses.Active
            };

            _loggingMarketDataProvider.WithdrawOrder(order);

            return true;
        }

        public LimitOrder ReplaceOrder(Securities security, long orderNumber, OrderParams orderTradeParams)
        {
            var wasOrderKilled = KillOrder(security, orderNumber);

            if (wasOrderKilled  == false)
            {
                _logger.Log(String.Format("ReplaceOrder: order with number: {0} wasn't killed.", orderNumber), LogMessageTypes.Error);

                return new LimitOrder
                {
                    DateTime = DateTime.Now,
                    OrderNumber = orderNumber,
                    Status = OrderStatuses.NotRegistered
                };
            }

            return CreateOrder(security, orderTradeParams);                                        
        }
    }
}