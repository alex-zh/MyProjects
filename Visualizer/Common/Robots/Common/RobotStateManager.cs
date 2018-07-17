using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Logger;

namespace Common.Robots.Common
{
    public class RobotStateManager
    {
        private readonly IRobotState _robotState;
        private readonly ILogger _logger;

        public event EventHandler StateChanged;

        private readonly object _lock = new object();

        public RobotStateManager(IRobotState robotState, ILogger logger)
        {
            _robotState = robotState;
            _logger = logger;
        }

        public IRobotState State => _robotState;

        public void AddSize(int addSize)
        {
            _robotState.AddSize(addSize);

            OnRobotStateChanged(EventArgs.Empty);
        }

        public IEnumerable<LimitOrder> GetOrders()
        {
            return _robotState.GetOrders();
        }

        public void UpdateOrder(LimitOrder order)
        {
            _robotState.AddOrUpdateOrder(order);

            OnRobotStateChanged(EventArgs.Empty);
        }

        public void RemoveOrder(string orderId)
        {
            _robotState.RemoveOrder(orderId);

            OnRobotStateChanged(EventArgs.Empty);
        }

        public void RemoveOrders(IEnumerable<string> ordersIds)
        {
            _robotState.RemoveOrders(ordersIds);

            OnRobotStateChanged(EventArgs.Empty);
        }

        public void CleanOrders()
        {
            lock (_lock)
            {
                var innerOrders = _robotState.GetOrders();

                var completedAndWithdrawnOrdersIds = innerOrders
                    .Where(x => x.Status == OrderStatuses.Completed || x.Status == OrderStatuses.Withdrawn)
                    .Select(x => x.OrderId).ToList();

                RemoveOrders(completedAndWithdrawnOrdersIds);
            }
        }

        public void Synchronize(List<RobotTrade> marketTrades)
        {
            lock (_lock)
            {
                var innerOrders = _robotState.GetOrders()
                                             .Where(x => x.Status != OrderStatuses.Withdrawn && x.Status != OrderStatuses.Withdrawn).ToList();

                var robotTrades = marketTrades.Where(t => innerOrders.Any(o => o.OrderId == t.OrderId));
                var size = robotTrades.Sum(x => x.Size * x.Side.ToInt());

                if (Math.Abs(_robotState.Size - size) > double.Epsilon)
                {
                    _robotState.SetSize(size);
                    OnRobotStateChanged(EventArgs.Empty);
                }
            }
        }

        public void Synchronize(List<LimitOrder> marketOrders)
        {
            bool stateChanged = false;

            lock (_lock)
            {
                var innerOrders = _robotState.GetOrders().ToList();

                foreach (var robotOrder in innerOrders)
                {
                    var marketOrder = marketOrders.FirstOrDefault(x => x.OrderId == robotOrder.OrderId);

                    if (marketOrder == null)
                    {
                        LogMissedMarketOrderError(robotOrder);
                    }
                    else if (robotOrder.Status != marketOrder.Status ||
                             robotOrder.DateTime != marketOrder.DateTime ||
                             Math.Abs(robotOrder.Balance - marketOrder.Balance) > double.Epsilon)
                    {
                        robotOrder.Status = marketOrder.Status;
                        robotOrder.DateTime = marketOrder.DateTime;
                        robotOrder.Balance = marketOrder.Balance;

                        stateChanged = true;
                    }
                }
            }

            if (stateChanged)
            {
                OnRobotStateChanged(EventArgs.Empty);
            }
        }

        private void OnRobotStateChanged(EventArgs args)
        {
            StateChanged?.Invoke(this, args);
        }

        private void LogMissedMarketOrderError(LimitOrder robotOrder)
        {
            _logger.Error($@"Order with number {robotOrder.SystemOrderId
                              } is present in robot orders collection but not present in quik.
                                                    The reason can be that order is registered but synchronization didn't take it yet ");
        }
    }
}
