using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Robots.Common
{
    public class RobotState : IRobotState
    {
        private readonly object _lock = new object();

        private double _size;
        private readonly List<LimitOrder> _orders;

        public RobotState(double size, List<LimitOrder> orders)
        {
            _size = size;
            _orders = orders;
        }

        public double Size
        {
            get
            {
                lock (_lock)
                    return _size;
            }
        }

        public PositionStates PositionState => (Math.Abs(Size) < double.Epsilon) ? PositionStates.NotOpen : PositionStates.Open;

        public IEnumerable<LimitOrder> GetOrders()
        {
            return _orders;
        }

        public void SetSize(double size)
        {
            lock (_lock)
            {
                _size = size;
            }
        }

        public void AddSize(double addSize)
        {
            lock (_lock)
            {
                _size += addSize;
            }
        }

        public void AddOrUpdateOrder(LimitOrder order)
        {
            lock (_lock)
            {
                var robotOrder = _orders.FirstOrDefault(x => x.OrderId == order.OrderId);

                if (order != null)
                {
                    _orders.Remove(robotOrder);
                }

                _orders.Add(order);
            }
        }

        public void RemoveOrder(string orderId)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(x => x.OrderId == orderId);

                if (order != null)
                    _orders.Remove(order);
            }
        }

        public void RemoveOrders(IEnumerable<string> orderIds)
        {
            lock (_lock)
            {
                foreach (var orderId in orderIds)
                {
                    var order = _orders.FirstOrDefault(x => x.OrderId == orderId);
                    _orders.Remove(order);
                }
            }
        }

    }
}