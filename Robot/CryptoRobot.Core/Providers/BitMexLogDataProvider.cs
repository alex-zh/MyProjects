using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Robot;
using Robot.Core.Classes;
using Robot.Core.Keepers;
using Robot.Quik2Net;

namespace Robot.Core.Providers
{
    public class BitMexLogDataProvider: ICryptoDataProvider
    {
        private List<LimitedOrder> _orders = new List<LimitedOrder>();

        private readonly QuikDdeServer _ddeServer;

        public BitMexLogDataProvider(IEnumerable<LimitedOrder> initialOrders)
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
        /// ������������������ ������, ����� ���-�� ���������, ����������� ��� �����������, ��� ���.
        /// ����� �� ��������� ������ �����, ������ ��� ������ �� ������� ������. 
        /// ��� ��� ��� ���-����� �������������, �������������� �������� �� ���������.
        /// </summary>
        private void UpdateOrders(Securities security, List<Candle> candles)
        {
            var isAtLeastOneCompleted = false;

            foreach (var order in _orders)
            {
                if(order.Security != security) continue;
                if(order.Status != OrderStatuses.Active) continue;

                /* ����� ����� ���� ������, 
                   ��� ��� ���� ����� ������� �����, ���������, ������ ����� ��� ���� ���������������, 
                   � ����� �� ���� ��������������� �������� ����� �� ��� ����� ������� �����������, ���� � ����� �� �� �� ���������� */
                var dateTimeWithMinutesPrecision = order.DateTime.ToMinutesPrecision();

                var candlesForOrder = candles.Where(x => x.Date >= dateTimeWithMinutesPrecision).ToList();

                if (candlesForOrder.Any())
                {
                    if ((order.Params.Direction == Direction.Up && order.Params.Price > candlesForOrder.Min(x => x.Low)) ||
                        (order.Params.Direction == Direction.Down && order.Params.Price < candlesForOrder.Max(x => x.High)))
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
        /// ������� �� OperationalDataProvider ��������� �����.
        /// ��� ��� ������� ������ �� ������ �� ������, ����� ��������� ������, 
        /// ������� ���������� ��� ����������� ��, � ��� ���.
        /// </summary>
        public IEnumerable<LimitedOrder> GetLimitedOrders()
        {
            return _orders;
        }

        /// <summary>
        /// ���������� QuikDataExecutor. 
        /// ��� ��� ��������� ������ Quik �� ����������, ����� ��������� �  ����������� ������ ������� �������.
        /// ����� ������ �����������, ��� ��� ��� State ��� ����� ������ �� Quik � ������ �� ��� ������ ���� ������ ������
        /// </summary>
        public void AddOrder(LimitedOrder order)
        {
            _orders.Add(order.Clone());

            OnOrdersChanged(EventArgs.Empty);
        }

        /// <summary>
        /// ���������� QuikDataExecutor.
        /// ��� ��� ��������� ������ Quik �� ����������, ����� ��������� �  ����������� ������ ������� �������.
        /// </summary>
        public void WithdrawOrder(LimitedOrder order)
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