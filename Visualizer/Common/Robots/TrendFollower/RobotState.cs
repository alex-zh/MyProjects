using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Common.Robots.TrendFollower
{
    public class RobotState : IRobotState
    {
        private readonly object _lock = new Object();

        private int _size;
        private readonly Dictionary<RobotOrderTypes, LimitOrder> _orders;

        public RobotState(int size, Dictionary<RobotOrderTypes, LimitOrder> orders)
        {
            _size = size;
            _orders = new Dictionary<RobotOrderTypes, LimitOrder>(orders);
        }

        public int Size
        {
            get
            {
                lock (_lock)
                    return _size;
            }
        }

        public Direction Direction => (Size > 0) ? Direction.Buy: Direction.Sell;

        public IEnumerable<LimitOrder> Orders => _orders.Values;

        public IEnumerable<LimitOrder> GetOrders()
        {
            return _orders.Select(x => x.Value);
        }

        public IEnumerable<LimitOrder> GetOrdersByType(RobotOrderTypes orderType)
        {
            return _orders.Where(x => x.Key == orderType).Select(x=>x.Value);
        }

        public void AddSize(int addSize)
        {
            lock (_lock)
            {
                _size += addSize;
            }
        }


        public bool HasPositionOrders()
        {
            lock (_lock)
            {
                return _orders.ContainsKey(RobotOrderTypes.OpenPosition) ||
                       _orders.ContainsKey(RobotOrderTypes.ClosePosition);
            }
        }

        public LimitOrder Get(RobotOrderTypes orderType)
        {
            lock (_lock)
            {
                LimitOrder order;
                _orders.TryGetValue(orderType, out order);

                return order;
            }
        }


        public void AddOrUpdateOrder(LimitOrder order)
        {
            lock (_lock)
            {
                var orderType = order.RobotOrderType;

                if (_orders.ContainsKey(orderType))
                {
                    _orders[orderType] = order;
                }
                else
                {
                    _orders.Add(orderType, order);
                }
            }

        }

        public void RemoveOrder(long orderNumber)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(x => x.Value.OrderNumber == orderNumber);

                if (order.Value != null)
                    _orders.Remove(order.Key);
            }
        }

        public void RemoveOrders(IEnumerable<long> ordersNumbers)
        {
            lock (_lock)
            {
                foreach (var orderNumber in ordersNumbers)
                {
                    var order = _orders.FirstOrDefault(x => x.Value.OrderNumber == orderNumber);
                    _orders.Remove(order.Key);
                }
            }
        }

    }
}