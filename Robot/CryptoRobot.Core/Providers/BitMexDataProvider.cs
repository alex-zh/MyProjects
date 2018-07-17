using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Robot;
using Robot.Core.Classes;
using Robot.Core.Keepers;
using Robot.Quik2Net;

namespace Robot.Core.Providers
{
    public class BitMexDataProvider : ICryptoDataProvider
    {        
        private readonly QuikDdeServer _ddeServer;        

        public BitMexDataProvider()
        {            
            _ddeServer = DdeServerKeeper.Get();
        }

        public event EventHandler<EventArgs> TimeChanged
        {
            add { _ddeServer.TimeChanged += value; }
            remove { _ddeServer.TimeChanged -= value; }
        }

        public event EventHandler<EventArgs> LimitedOrdersChanged
        {
            add { _ddeServer.LimitedOrdersChanged += value; }
            remove { _ddeServer.LimitedOrdersChanged -= value; }
        }

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

            return LastCandleRemover.Remove(rawCandles);
        }

        public IEnumerable<RobotTrade> GetTrades()
        {
            return _ddeServer.GetTrades();
        }

        public IEnumerable<LimitedOrder> GetLimitedOrders()
        {
            return _ddeServer.GetLimitedOrders();
        }
    }
}