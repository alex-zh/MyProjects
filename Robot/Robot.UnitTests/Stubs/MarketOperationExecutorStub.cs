using System;
using System.Collections.Generic;
using Common.Robots.Common;
using Robot.Core;

namespace Robot.UnitTests.Stubs
{
    public class MarketOperationExecutorStub : IMarketOperationExecutor
    {
        private readonly MarketDataProviderStub _marketDataProviderStub;

        public MarketOperationExecutorStub(MarketDataProviderStub marketDataProviderStub)
        {
            _marketDataProviderStub = marketDataProviderStub;
        }

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
                OrderId = Guid.NewGuid().ToString(),
                Size = orderParams.Size,
                Price = orderParams.Price,
                OrderSide = orderParams.Side,                
                Status = OrderStatuses.Active,
                Balance = orderParams.Size
            };

            _marketDataProviderStub.AddToActual(order);

            return order;
        }

      
        public bool DeleteOrder(string orderId)
        {
            var order = new LimitOrder
            {
                OrderId = orderId,
                Status = OrderStatuses.Withdrawn,
            };

            _marketDataProviderStub.AddToActual(order);

            return true;
        }

        public LimitOrder ReplaceOrder(string orderId, OrderParams orderParams)
        {
            return CreateOrder("", orderParams);
        }
       
    }
}