using System.Collections.Generic;
using Common.Robots.Common;

namespace Robot.Core
{
    public interface IMarketOperationExecutor
    {
        bool DeleteOrder(string orderId);
        bool DeleteOrders(List<string> orderIds);

        LimitOrder CreateOrder(string symbol, OrderParams orderParams);
        List<LimitOrder> CreateOrders(string symbol, List<OrderParams> orders);

        LimitOrder ReplaceOrder(string orderId, OrderParams orderParams);
    }
}