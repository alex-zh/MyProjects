using System;
using System.Collections.Generic;

namespace Common.Robots.Common
{
    public class OrdersCompletedEventArgs: EventArgs
    {
        public OrdersCompletedEventArgs(IEnumerable<LimitOrder> completedOrders)
        {
            Orders = completedOrders;
        }

        public IEnumerable<LimitOrder> Orders { get; }
    }
}