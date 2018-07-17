using System;
using System.Linq;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Robot.Core.Providers;

namespace Robot.Core
{
    public class QuikTradeExecutorForLogging : IQuikTradeExecutor
    {        
        private static int _orderNumber = 0;
        private readonly BitMexLogDataProvider _loggingQuikDataProvider;
        private readonly ILogger _logger;
        
        public QuikTradeExecutorForLogging(BitMexLogDataProvider loggingQuikDataProvider, ILogger logger)
        {
            _loggingQuikDataProvider = loggingQuikDataProvider;
            _logger = logger;
        }

        public LimitedOrder CreateOrder(Securities security, OrderParams orderParams)
        {
            var order = new LimitedOrder
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
                var lastCandle = _loggingQuikDataProvider.GetLastCandle(Securities.Si1Min);

                order.Params.Price = lastCandle.Close;
            }

            _loggingQuikDataProvider.AddOrder(order);

            return order;
        }

        public bool KillOrder(Securities security, long orderNumber)
        {
            var order = new LimitedOrder
            {
                DateTime = DateTime.Now,
                OrderNumber = orderNumber,
                Status = OrderStatuses.Active
            };

            _loggingQuikDataProvider.WithdrawOrder(order);

            return true;
        }

        public LimitedOrder ReplaceOrder(Securities security, long orderNumber, OrderParams orderTradeParams)
        {
            var wasOrderKilled = KillOrder(security, orderNumber);

            if (wasOrderKilled  == false)
            {
                _logger.Log(String.Format("ReplaceOrder: order with number: {0} wasn't killed.", orderNumber), LogMessageTypes.Error);

                return new LimitedOrder
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