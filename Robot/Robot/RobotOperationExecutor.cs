using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.Core
{
    public class RobotOperationExecutor
    {
        private readonly Securities _security;
        private readonly IMarketOperationExecutor _marketOperationExecutor;
        private readonly RobotStateManager _stateManager;

        public RobotOperationExecutor(Securities security,
                                      RobotStateManager stateManagerManager,
                                      IMarketOperationExecutor marketOperationExecutor)
        {
            _security = security;
            _stateManager = stateManagerManager;
            _marketOperationExecutor = marketOperationExecutor;
        }        
        
        /// <summary>
        /// Creates or updates or kills the order depening on order parameters
        /// Заменяет лимитированную заявку. 
        /// Операция состоит из двух частей:
        ///  - Снятие существующей
        ///  - Установка новой
        /// За счет этого может произойти ситуация, когда зарегистрированна заявка на снятие, но не была выполнена.
        /// При этом регистрируется новая заявка. 
        /// Пока считаем эту ситуацию редкой (невозвожной), т.к снятие происходит достаточно быстро, по сути в момент регистрации заявки 
        /// </summary>
        public LimitOrderOld SendOrder(RobotOrderTypes orderType, OrderParams orderParams)
        {
            LimitOrderOld result = null;

            var robotOrder = _stateManager.GetOrdersByType(orderType).FirstOrDefault();
           
            if (robotOrder != null && robotOrder.IsActive && orderParams != null)
            {
                var robotOrderType = robotOrder.RobotOrderType;

                result = _marketOperationExecutor.ReplaceOrder(_security, robotOrder.OrderNumber, orderParams);
                result.RobotOrderType = robotOrderType;
            }
            else if ((robotOrder == null ||  robotOrder.IsActive == false) && orderParams != null)
            {
                result = _marketOperationExecutor.CreateOrder(_security, orderParams);
                result.RobotOrderType = orderType;
            }

            if (result != null && result.IsActive)
            {
                _stateManager.UpdateOrder(result);
            }

            return result;
        }


        public void WithdrawOrder(RobotOrderTypes orderType)
        {
            var currentOrder = _stateManager.GetOrdersByType(orderType).FirstOrDefault();

            if (currentOrder != null && currentOrder.IsActive)
            {
                var wasOrderKilled = _marketOperationExecutor.KillOrder(_security, currentOrder.OrderNumber);

                if (wasOrderKilled)
                {
                    _stateManager.RemoveOrder(currentOrder.OrderNumber);
                }
            }
        }

    }
}