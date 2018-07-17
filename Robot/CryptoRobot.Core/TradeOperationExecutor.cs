using System;
using Common.Classes.Robot;
using Robot.Core.Classes;
using Robot.Trans2Quik.Entities.Transaction.Order;

namespace Robot.Core
{
    /// <summary>
    /// Исполнитель торговых операций. 
    /// Инджектим IQuikProxy который непосредсвенно отвечает за исполнение операций
    /// </summary>
    public class TradeOperationExecutor
    {
        private readonly Securities _security;
        private readonly IQuikTradeExecutor _quikTradeExecutor;
        private readonly RobotState _robotState;

        public TradeOperationExecutor(Securities security,
                                      RobotState state,
                                      IQuikTradeExecutor quikTradeExecutor)
        {
            _security = security;
            _robotState = state;
            _quikTradeExecutor = quikTradeExecutor;
        }

        public LimitedOrder RegisterOrder(RobotOrderTypes robotOrderType, OrderParams orderParams)
        {
            ValidateOrderParams(orderParams);

            var order = _quikTradeExecutor.CreateOrder(_security, orderParams);

            order.RobotOrderType = robotOrderType;

            if (order.IsActive)
            {
                _robotState.UpdateOrder(order);
            }

            return order;
        }

        private void ValidateOrderParams(OrderParams orderParams)
        {
            if (orderParams == null)
            {
                throw new ArgumentNullException("orderParams");
            }

            if (orderParams.Size == 0)
            {
                throw new ArgumentException("Attempt to register order with zero size.");
            }
        }

        public void KillOrder(RobotOrderTypes orderType)
        {
            var currentOrder = _robotState.Get(orderType);

            if (currentOrder.IsActive)
            {
                var wasOrderKilled = _quikTradeExecutor.KillOrder(_security, currentOrder.OrderNumber);

                if (wasOrderKilled)
                {
                    _robotState.RemoveOrder(orderType, currentOrder.OrderNumber);
                }
            }
        }

        /// <summary>
        /// Заменяет лимитированную заявку. 
        /// Операция состоит из двух частей:
        ///  - Снятие существующей
        ///  - Установка новой
        /// За счет этого может произойти ситуация, когда зарегистрированна заявка на снятие, но не была выполнена.
        /// При этом регистрируется новая заявка. 
        /// Пока считаем эту ситуацию редкой (невозвожной), т.к снятие происходит достаточно быстро, по сути в момент регистрации заявки 
        /// </summary>
        public LimitedOrder UpdateOrder(RobotOrderTypes orderType, OrderParams orderParams)
        {
            LimitedOrder result = null;

            var currentOrder = _robotState.Get(orderType);

            if (currentOrder.IsActive && orderParams == null)
            {
                var wasOrderKilled = _quikTradeExecutor.KillOrder(_security, currentOrder.OrderNumber);

                if (wasOrderKilled)
                {
                    _robotState.RemoveOrder(orderType, currentOrder.OrderNumber);
                }
            }
            else if (currentOrder.IsActive && orderParams != null)
            {
                //TODO: Логика по обновлению должна быть более умной, если order не меняется не имеет смысла посылать транзакцию и создавать нагрузку                

                var robotOrderType = currentOrder.RobotOrderType;

                result = _quikTradeExecutor.ReplaceOrder(_security, currentOrder.OrderNumber, orderParams);
                result.RobotOrderType = robotOrderType;
            }
            else if (currentOrder.IsActive == false && orderParams != null)
            {
                result = _quikTradeExecutor.CreateOrder(_security, orderParams);
                result.RobotOrderType = orderType;
            }

            if (result != null && result.IsActive)
            {
                _robotState.UpdateOrder(result);
            }

            return result;
        }        
    }
}