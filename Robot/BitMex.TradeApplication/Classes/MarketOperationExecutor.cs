using System;
using System.Collections.Generic;
using System.Linq;
using BitMexConnector;
using BitMexConnector.Tables.Classes;
using Common.Classes.Logger;
using Common.Robots.Common;
using Robot.Core;

namespace BitMex.TradeApplication.Classes
{
    internal class MarketOperationExecutor : IMarketOperationExecutor
    {
        private readonly ILogger _logger;
        private readonly BitMexOperationExecutor _marketOperationExecutor;

        public MarketOperationExecutor(ILogger logger)
        {
            _logger = logger;
            _marketOperationExecutor = new BitMexOperationExecutor(logger);
        }

        public bool DeleteOrder(string orderId)
        {
            _logger.Info("It's better to use bulk delete order operation");
            return _marketOperationExecutor.DeleteOrder(orderId).Result;
        }

        public bool DeleteOrders(List<string> orderIds)
        {
            if (orderIds.Any())
            {
                return _marketOperationExecutor.DeleteOrders(orderIds).Result;
            }

            return true;
        }

        LimitOrder IMarketOperationExecutor.CreateOrder(string symbol, OrderParams orderParams)
        {
            _logger.Info("It's better to use bulk create order operation");

            var order = new BitMexLimitOrder
            {
                Symbol = symbol,
                OrderId = Guid.NewGuid().ToString(),
                Price = orderParams.Price,
                Size = orderParams.Size,
                Side = orderParams.Side,
                LeftSize = orderParams.Size,
                OrderStatus = OrderStatuses.Active,
                OrderType = OrderTypes.Limit,
            };

            var haveBeenSet = _marketOperationExecutor.SetOrder(order).Result;

            if (haveBeenSet)
            {
                return order.ToLimitOrder();
            }

            return null;           
        }

        List<LimitOrder> IMarketOperationExecutor.CreateOrders(string symbol, List<OrderParams> orders)
        {
            var bitMexOrders = orders.Select(orderParams => new BitMexLimitOrder
            {
                Symbol = symbol,
                OrderId = Guid.NewGuid().ToString(),
                DateTime = new DateTime(1970,1, 1),
                Price = orderParams.Price,
                Size = orderParams.Size,
                Side = orderParams.Side,
                LeftSize = orderParams.Size,
                OrderStatus = OrderStatuses.Active,
                OrderType = OrderTypes.Limit,
            }).ToList();

            var haveBeenSet = _marketOperationExecutor.SetOrders(bitMexOrders).Result;

            if (haveBeenSet)
            {
                return bitMexOrders.Select(x => x.ToLimitOrder()).ToList();
            }

            return new List<LimitOrder>();
        }

        LimitOrder IMarketOperationExecutor.ReplaceOrder(string orderId, OrderParams orderParams)
        {
            throw new NotSupportedException();
        }
    }
}