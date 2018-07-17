using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Core.Classes;
using Robot.Core.Keepers;
using Robot.Core.MarketDataProviders;
using Robot.Quik2Net;

namespace Robot.Core.Providers.Quik
{
    public class QuikLoggingMarketDataProvider: IMarketDataProvider
    {
        private List<LimitOrder> _orders = new List<LimitOrder>();

        private readonly QuikDdeServer _ddeServer;

        public QuikLoggingMarketDataProvider(IEnumerable<LimitOrder> initialOrders)
        {
            _ddeServer = DdeServerKeeper.Get();

            foreach (var initialOrder in initialOrders)
            {
                _orders.Add(initialOrder.Clone());    
            }            
        }

      public event EventHandler<EventArgs> TimeChanged
        {
            add { _ddeServer.TimeChanged += value; }
            remove { _ddeServer.TimeChanged -= value; }
        }

        public event EventHandler<EventArgs> LimitedOrdersChanged;

        public event EventHandler<EventArgs> DataChanged
        {
            add { _ddeServer.DataChanged += value; }
            remove { _ddeServer.DataChanged -= value; }
        }

        public DateTime GetServerTime()
        {
            return _ddeServer.GetServerTime();
        }

        /// <summary>
        /// Returns the latest available candle. It might live only for 1 second yet.
        /// </summary>
        public Candle GetLastCandle(Securities security)
        {
            var rawCandles = _ddeServer.GetCandles(RobotSettings.GetSecurityName(security)).ToList();

            return rawCandles.LastOrDefault();
        }

        public IEnumerable<Candle> GetCandles(Securities security, int requestedCount)
        {
            var rawCandles = _ddeServer.GetCandles(RobotSettings.GetSecurityName(security)).ToList();

            var skipCount = Math.Max(0, rawCandles.Count - requestedCount);

            rawCandles = rawCandles.Skip(skipCount).ToList();

            var candles =  LastCandleRemover.Remove(rawCandles);            

            UpdateOrders(security, candles);

            return candles;
        }

        public IEnumerable<RobotTrade> GetTrades()
        {
            return _ddeServer.GetTrades();
        }

        /// <summary>
        /// «арегестрированные ордера, нужно как-то обновл€ть, проставл€ть что исполнилось, что нет.
        /// „тобы не создавать лишнюю св€зь, удобно это делать на запросе свечей. 
        /// “ак как они все-равно запрашиваютс€, дополнительных расходов не проиходит.
        /// </summary>
        private void UpdateOrders(Securities security, List<Candle> candles)
        {
            var isAtLeastOneCompleted = false;

            foreach (var order in _orders)
            {
                if(order.Security != security) continue;
                if(order.Status != OrderStatuses.Active) continue;

                /* здесь может быть ошибка, 
                   так как цена могла сходить вверх, вернутьс€, дальше ордер мог быть зарегистрирован, 
                   в итоге за счет первоначального движени€ вверх мы его будем считать исполненным, хот€ в жизни он бы не исполнилс€ */
                var dateTimeWithMinutesPrecision = order.DateTime.ToMinutesPrecision();

                var candlesForOrder = candles.Where(x => x.Date >= dateTimeWithMinutesPrecision).ToList();

                if (candlesForOrder.Any())
                {
                    if ((order.Params.Direction == Direction.Buy && order.Params.Price > candlesForOrder.Min(x => x.Low)) ||
                        (order.Params.Direction == Direction.Sell && order.Params.Price < candlesForOrder.Max(x => x.High)))
                    {
                        isAtLeastOneCompleted = true;
                        order.Status = OrderStatuses.Completed;
                    }
                }
            }

            if (isAtLeastOneCompleted)
            {
                OnOrdersChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// ќтличие от OperationalDataProvider находитс€ здесь.
        /// “ак как реально ордера не уход€т на сервер, здесь находитс€ логика, 
        /// котора€ определ€ет что исполнилось бы, а что нет.
        /// </summary>
        public IEnumerable<LimitOrder> GetLimitedOrders()
        {
            return _orders;
        }

        /// <summary>
        /// ¬ызываетс€ QuikDataExecutor. 
        /// “ак как реального вызова Quik не происходит, нужно сохран€ть и  эмулировать запрос текущих ордеров.
        /// ¬ажно именно клонировать, так как дл€ PositionState это будут ордера из Quik и именно по ним должен быть апдейт стейта
        /// </summary>
        public void AddOrder(LimitOrder order)
        {
            _orders.Add(order.Clone());

            OnOrdersChanged(EventArgs.Empty);
        }

        /// <summary>
        /// ¬ызываетс€ QuikDataExecutor.
        /// “ак как реального вызова Quik не происходит, нужно сохран€ть и  эмулировать запрос текущих ордеров.
        /// </summary>
        public void WithdrawOrder(LimitOrder order)
        {
            var registeredOrder = _orders.First(x => x.OrderNumber == order.OrderNumber);
            registeredOrder.Status = OrderStatuses.Withdrawn;

            OnOrdersChanged(EventArgs.Empty);
        }

        private void OnOrdersChanged(EventArgs args)
        {
            if (LimitedOrdersChanged != null)
            {
                LimitedOrdersChanged(this, args);
            }
        }

    }
}