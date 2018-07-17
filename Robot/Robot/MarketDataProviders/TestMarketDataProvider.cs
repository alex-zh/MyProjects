using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.Core.MarketDataProviders
{
    public class TestMarketDataProvider : IMarketDataProvider
    {   
        private readonly Dictionary<string, IEnumerable<Candle>> _candlesDictionary = new Dictionary<string, IEnumerable<Candle>>();
        private IEnumerable<LimitOrder> _orders;

        public event EventHandler<EventArgs> TimeChanged;
        public event EventHandler<EventArgs> DataChanged;
        public event EventHandler<EventArgs> OrdersChanged;

        public DateTime GetServerTime()
        {
            throw new NotImplementedException();
        }

        public Quote GetLastQuote(string symbol)
        {
            return new Quote();
        }

        public void InitCandles(string symbol, IEnumerable<Candle> candles)
        {
            _candlesDictionary[symbol] = candles;
        }

        public Candle GetLastCandle(string symbol)
        {
            var rawCandles = _candlesDictionary[symbol].ToList();

            return rawCandles.Last();
        }

        public IEnumerable<Candle> GetCandles(string symbol, int requestedCount)
        {
            var rawCandles = _candlesDictionary[symbol].ToList();

            var skipCount = Math.Max(0, rawCandles.Count - requestedCount);

            rawCandles = rawCandles.Skip(skipCount).ToList();

            return rawCandles;
        }

        public IEnumerable<RobotTrade> GetTrades()
        {
            return new List<RobotTrade>();
        }

        public IEnumerable<RobotTrade> GetTrades(string orderId)
        {
            return new List<RobotTrade>();
        }

        public IEnumerable<LimitOrder> GetLimitOrders()
        {
            return _orders;
        }

        public void SetupOrdersAndMoveThemToCompletedState(IEnumerable<LimitOrder> orders, Candle currentCandle, bool useMakerOrder = false)
        {
            _orders = orders.Select(x => x.Clone()).ToList();

            var wasOrderChanged = false;

            foreach (var order in _orders)
            {
                if (Math.Abs(order.Price) < double.Epsilon)
                {
                    // can be directly closed only if it is "at market" order
                    order.Balance = 0;
                    order.Status = OrderStatuses.Completed;

                    wasOrderChanged = true;
                }                
                else if (!useMakerOrder && (
                    (order.OrderSide == OrderSides.Buy && order.Price > currentCandle.Low) ||
                    (order.OrderSide == OrderSides.Sell && order.Price < currentCandle.High)))
                {
                    if (order.OrderSide == OrderSides.Buy)
                    {
                        if (order.Price > currentCandle.Open)
                        {
                            order.Price = currentCandle.Open;
                        }
                    }
                    else if (order.OrderSide == OrderSides.Sell)
                    {
                        if (order.Price < currentCandle.Open)
                        {
                            order.Price = currentCandle.Open;
                        }
                    }

                    order.Balance = 0;
                    order.Status = OrderStatuses.Completed;
                    wasOrderChanged = true;
                }
                else if (useMakerOrder && (
                   (order.OrderSide == OrderSides.Buy && order.Price > currentCandle.Low && currentCandle.Low < currentCandle.Open * (1 - 0.0001)) ||
                    (order.OrderSide == OrderSides.Sell && order.Price < currentCandle.High && currentCandle.High > currentCandle.Open * (1.0001))))
                {
                    if (order.OrderSide == OrderSides.Buy)
                    {
                        order.Price = Math.Min(order.Price, currentCandle.Open * (1 - 0.0001));
                    }
                    else if (order.OrderSide == OrderSides.Sell)
                    {
                        order.Price = Math.Max(order.Price, currentCandle.Open * (1 + 0.0001));
                    }

                    order.Balance = 0;
                    order.Status = OrderStatuses.Completed;
                    wasOrderChanged = true;
                }                
            }

            if (wasOrderChanged)
            {
                OrdersChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateKilledOrder(IEnumerable<LimitOrder> orders, LimitOrder killedOrder, bool triggerOrderChangedEvent)
        {
            _orders = orders.Select(x => x.Clone()).ToList();

            var order = _orders.FirstOrDefault(x => x.OrderId == killedOrder.OrderId);

            if (order != null)
            {
                order.Status = OrderStatuses.Withdrawn;
            }

            if (triggerOrderChangedEvent)
            {
                OrdersChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> TradesChanged;
    }
}