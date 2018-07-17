using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Robots.Common;
using Robot.Core.MarketDataProviders;

namespace Robot.UnitTests.Stubs
{
    public class MarketDataProviderStub : IMarketDataProvider
    {
        private readonly List<LimitOrder> _orders = new List<LimitOrder>();

        public event EventHandler<EventArgs> TimeChanged;
        public event EventHandler<EventArgs> DataChanged;
        public event EventHandler<EventArgs> OrdersChanged;

        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }

        public Quote GetLastQuote(string symbol)
        {
            return new Quote();
        }

        public Candle GetLastCandle(string symbol)
        {
            return new Candle();
        }

        public IEnumerable<Candle> GetCandles(string symbol, int count)
        {
            return new Candle[0];
        }

        
        public IEnumerable<RobotTrade> GetTrades()
        {
            return new List<RobotTrade>();
        }

        public IEnumerable<LimitOrder> GetLimitOrders()
        {
            return _orders;
        }

        public void TriggerChangedEvent()
        {
            if (OrdersChanged != null)
            {
                OrdersChanged(this, EventArgs.Empty);
            }
        }

        public void ClearAll()
        {
            _orders.Clear();
        }

        public void AddToActual(LimitOrder order)
        {
            var existentOrder = _orders.FirstOrDefault(x => x.OrderId == order.OrderId);

            if (existentOrder != null)
            {
                _orders.Remove(existentOrder);
            }

            _orders.Add(order.Clone());
        }

        public void ChangeSize(int index, double size)
        {
            _orders[index].Size = size;
        }

        public void MoveToStatus(int index, OrderStatuses status)
        {
            _orders[index].Status = status;
        }

        public void MoveAllTo(OrderStatuses status)
        {
            foreach (var order in _orders)
            {
                if (order.Status == OrderStatuses.Active)
                {
                    order.Balance = 0;
                    order.Status = status;
                }
            }
        }

        public IEnumerable<RobotTrade> GetTrades(string orderId)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> TradesChanged;
    }
}