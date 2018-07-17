using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;

namespace Common.Robots.Common
{
    public class RobotStateOld
    {
        private readonly object _lock = new Object();

        private double _size;
        private readonly Dictionary<RobotOrderTypes, LimitOrderOld> _orders;

        public RobotStateOld(double size, Dictionary<RobotOrderTypes, LimitOrderOld> orders)
        {
            _size = size;
            _orders = new Dictionary<RobotOrderTypes, LimitOrderOld>(orders);
        }

        public double Size
        {
            get
            {
                lock (_lock)
                    return _size;
            }
        }

        public Direction Direction => (Size > 0) ? Direction.Buy: Direction.Sell;

        public IEnumerable<LimitOrderOld> Orders => _orders.Values;

        public IEnumerable<LimitOrderOld> GetOrders()
        {
            return _orders.Select(x => x.Value);
        }

        public IEnumerable<LimitOrderOld> GetOrdersByType(RobotOrderTypes orderType)
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

        public LimitOrderOld Get(RobotOrderTypes orderType)
        {
            lock (_lock)
            {
                LimitOrderOld orderOld;
                _orders.TryGetValue(orderType, out orderOld);

                return orderOld;
            }
        }


        public void AddOrUpdateOrder(LimitOrderOld orderOld)
        {
            lock (_lock)
            {
                var orderType = orderOld.RobotOrderType;

                if (_orders.ContainsKey(orderType))
                {
                    _orders[orderType] = orderOld;
                }
                else
                {
                    _orders.Add(orderType, orderOld);
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