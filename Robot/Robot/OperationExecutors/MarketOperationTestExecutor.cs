using System;
using System.Collections.Generic;
using Common.Classes.General;
using Common.Robots.Common;

namespace Robot.Core.OperationExecutors
{
    public class MarketOperationTestExecutor : IMarketOperationExecutor
    {
        private Candle _currentCandle;
        public event EventHandler<LimitOrder> OrderKilled;

        public List<LimitOrder> CreateOrders(string symbol, List<OrderParams> orders)
        {
            throw new NotImplementedException();
        }

        public bool DeleteOrders(List<string> orderIds)
        {
            throw new NotImplementedException();
        }

        public LimitOrder CreateOrder(string symbol, OrderParams orderParams)
        {
            var order = new LimitOrder
            {
                Symbol = symbol,
                DateTime = _currentCandle.Date,
                Status = OrderStatuses.Active,
                Size = orderParams.Size,
                Price = orderParams.Price,
                OrderSide = orderParams.Side,                
                Balance = orderParams.Size,
                OrderId= Guid.NewGuid().ToString()
            };

            if (Math.Abs(orderParams.Price) < Double.Epsilon)
            {
                if (orderParams.Side == OrderSides.Buy)
                {
                    order.Price = _currentCandle.Open * (1 + 0.00001);
                }
                else
                {
                    order.Price = _currentCandle.Open * (1 - 0.00001);
                }
            }

            return order;
        }

        
        public bool DeleteOrder(string orderId)
        {
            var order = new LimitOrder
            {                
                DateTime = _currentCandle.Date,
                OrderId = orderId,
                Status = OrderStatuses.Active
            };

            OrderKilled?.Invoke(this, order);

            return true;
        }

        public LimitOrder ReplaceOrder(string orderId, OrderParams orderParams)
        {
            return CreateOrder("", orderParams);
        }

        public void SetCurrentCandle(Candle currentCandle)
        {
            _currentCandle = currentCandle;
        }
    }
}