using System;
using System.Globalization;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Robot.Core.Classes;
using Robot.Core.Keepers;
using Robot.Trans2Quik;
using Robot.Trans2Quik.Entities.Transaction;

namespace Robot.Core
{
    public class QuikTradeExecutor : IQuikTradeExecutor
    {
        private readonly ILogger _logger;
        private readonly Gateway _gateway;
        
        public QuikTradeExecutor(ILogger logger)
        {
            _logger = logger;            
            _gateway = QuikGatewayKeeper.Get();            
        }
       
        public LimitedOrder CreateOrder(Securities security, OrderParams orderParams)
        {
            var orderTradeParams = QuikProxyConverter.ToOrderTradeParams(security, orderParams);

            var result = Execute(() => _gateway.SendOrderSync(orderTradeParams));

            if (result.IsSuccess == false)
            {
                _logger.Log("Creating order fails: " + result.ResultMessage, LogMessageTypes.Error);
            }

            return new LimitedOrder
            {                
                Params = orderParams,
                DateTime = DateTime.Now,
                Balance = orderParams.Size,
                OrderNumber = result.OrderNumber,                
                Status = result.IsSuccess ? OrderStatuses.Active : OrderStatuses.NotRegistered
            };
        }

        public bool KillOrder(Securities security, long orderNumber)
        {
            var result = Execute(() => _gateway.KillOrderSync(QuikSecurityFactory.Get(security), orderNumber.ToString(CultureInfo.InvariantCulture)));

            return result.IsSuccess;
        }

        public LimitedOrder ReplaceOrder(Securities security, long orderNumber, OrderParams orderTradeParams)
        {
            var wasOrderKilled = KillOrder(security, orderNumber);

            if (wasOrderKilled == false)
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

        private TransactionCallResult Execute(Func<TransactionCallResult> func)
        {
            var result = func();

            LogResult(result);

            return result;
        }

        private void LogResult(TransactionCallResult result)
        {
            var messageType = result.IsSuccess ? LogMessageTypes.Info : LogMessageTypes.Warning;

            _logger.Log(result.ResultMessage, messageType);            
        }
    }
}