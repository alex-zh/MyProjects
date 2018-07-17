using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.Logger;
using Common.Robots.Common;

namespace Robot.Core
{
    public class NetThrowerRobotOperationExecutor
    {
        private readonly string _symbol;
        private readonly IMarketOperationExecutor _marketOperationExecutor;        
        private readonly RobotStateManager _stateManager;
        private readonly ILogger _logger;

        public NetThrowerRobotOperationExecutor(string symbol,
                                      RobotStateManager stateManagerManager,
                                      IMarketOperationExecutor marketOperationExecutor, 
                                      ILogger logger)
        {
            _symbol = symbol;
            _stateManager = stateManagerManager;
            _marketOperationExecutor = marketOperationExecutor;
            _logger = logger;
        }

        public void UpdateOrders(List<OrderParams> ordersParams)
        {
            var wasWithdrawnSuccessfully = WithdrawOrders();

            if (wasWithdrawnSuccessfully)
            {
                var orders = _marketOperationExecutor.CreateOrders(_symbol, ordersParams);

                foreach (var order in orders)
                {
                    if (order != null && order.IsActive())
                    {
                        _stateManager.UpdateOrder(order);
                    }
                }
            }
            else
            {
                _logger.Error("Orders haven't been updated because because previous ones haven't been withdrawn");
            }
        }

        private bool WithdrawOrders()
        {
            var orders = _stateManager.GetOrders().ToList();
            var orderIds = orders.Select(x => x.OrderId).ToList();
            var isSuccess = true;

            foreach (var orderId in orderIds)
            {
                isSuccess  = isSuccess && _marketOperationExecutor.DeleteOrder(orderId);
            }

            return isSuccess;            
        }
    }
}